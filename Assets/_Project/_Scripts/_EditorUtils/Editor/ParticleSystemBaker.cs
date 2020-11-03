using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class ParticleSystemBaker
{
    class ParticleIngredient
    {
        public Transform transform;
        public ParticleIngredient parent;

        public Material material;
        public Material trailsMaterial;
        public Mesh mesh;
        public Mesh trailsMesh;

        public GameObject generatedObject;
    }

    static int counterSaved = 0;

    static List<ParticleIngredient> ingredients = new List<ParticleIngredient>();

    [MenuItem("CONTEXT/ParticleSystem/Bake Into Mesh")]
    static void BakePS(MenuCommand command)
    {
        ingredients.Clear();

        ParticleSystem ps = (ParticleSystem)command.context;
        Transform t = ps.transform;
        InvestigateTransform(t, true);

        GameObject NewObjectForMesh(Mesh m, Material mat)
        {
            GameObject newObject = new GameObject();

            MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = m;

            MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = mat;

            return newObject;
        }

        string path = "";
        string key = counterSaved.ToString() + DateTime.Now.ToString("MMddyyyy_hhmmsstt");
        {
            path = Application.dataPath + "/GeneratedMeshes/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = "Assets/GeneratedMeshes";
        }

        string objPath = path + "/" + key + ".prefab";
        Debug.Log(objPath);
        GameObject wholeObj = new GameObject();
        GameObject p = PrefabUtility.SaveAsPrefabAssetAndConnect(wholeObj, objPath, InteractionMode.AutomatedAction);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Dictionary<ParticleIngredient, Transform> transformByParticleIngredient = new Dictionary<ParticleIngredient, Transform>();

        for (int i = 0; i < ingredients.Count; i++)
        {
            ParticleIngredient pi = ingredients[i];

            AssetDatabase.AddObjectToAsset(pi.mesh, AssetDatabase.GetAssetPath(p));

            GameObject obj = NewObjectForMesh(pi.mesh, pi.material);
            obj.transform.SetParent(wholeObj.transform);

            obj.name = "_Particles";

            //if(pi.parent != null)
            //    obj.transform.localPosition = pi.transform.localPosition;

            if (pi.trailsMesh != null)
            {
                AssetDatabase.AddObjectToAsset(pi.trailsMesh, AssetDatabase.GetAssetPath(p));
                GameObject trailObj = NewObjectForMesh(pi.trailsMesh, pi.trailsMaterial);
                trailObj.name = "_Trails";

                trailObj.transform.SetParent(obj.transform);
            }

            pi.generatedObject = obj;
            transformByParticleIngredient.Add(pi, obj.transform);
        }

        for (int i = 0; i < ingredients.Count; i++)
        {
            ParticleIngredient pi = ingredients[i];
            if(pi.parent != null)
            {
                if (transformByParticleIngredient.ContainsKey(pi.parent))
                {
                    pi.generatedObject.transform.SetParent(transformByParticleIngredient[pi.parent]);
                }
            }
        }

        PrefabUtility.ApplyPrefabInstance(wholeObj, InteractionMode.AutomatedAction);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        GameObject.DestroyImmediate(wholeObj);
    }

    static Matrix4x4 scaleaMatrix = default(Matrix4x4);

    static ParticleIngredient InvestigateTransform(Transform t, bool first = false)
    {
        ParticleIngredient ingredient = null;
        
        ParticleSystemRenderer renderer = t.GetComponent<ParticleSystemRenderer>();
        if (renderer)
        {
            ParticleSystem ps = t.GetComponent<ParticleSystem>();


            ingredient = new ParticleIngredient();
            ingredient.transform = t;

            Mesh newMesh = new Mesh();
            newMesh.name = "_Mesh";
            renderer.BakeMesh(newMesh, true);
            ingredient.mesh = newMesh;
            ingredient.material = renderer.sharedMaterial;

            if (ps.trails.enabled)
            {
                Mesh trailsMesh = new Mesh();
                trailsMesh.name = "_TrailsMesh";
                renderer.BakeTrailsMesh(trailsMesh, true);
                ingredient.trailsMesh = trailsMesh;
                ingredient.trailsMaterial = renderer.trailMaterial;
            }

            ingredients.Add(ingredient);

        }



        for (int i = 0; i < t.childCount; i++)
        {
            var childIngredient = InvestigateTransform(t.GetChild(i));

            if(ingredient != null && childIngredient != null)
                childIngredient.parent = ingredient;       
        }

        return ingredient;
    }
}
