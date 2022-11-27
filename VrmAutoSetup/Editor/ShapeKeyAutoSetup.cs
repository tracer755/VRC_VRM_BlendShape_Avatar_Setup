using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using PlasticPipe.Tube;
using UnityEngine.Networking.Types;
using VRM;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System;

public class ShapeKeyAutoSetup : EditorWindow
{
    GameObject Avatar;
    string SaveLocation;
    string name;
    Vector2 scrollPos;

    [MenuItem("Tools/Vrm/Auto setup shapekeys")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ShapeKeyAutoSetup));
    }


    private async void OnGUI()
    {
        //List<string> allowedBlendShapes = new List<string>();

        string[] arkit_BlendShapes = new string[] { "BrowDownLeft", "BrowDownRight", "BrowInnerUp", "BrowOuterUpLeft", "BrowOuterUpRight", "CheekPuff", "CheekSquintLeft", "CheekSquintRight", "EyeBlinkLeft", "EyeBlinkRight", "EyeLookDownLeft", "EyeLookDownRight", "EyeLookInLeft", "EyeLookInRight", "EyeLookOutLeft", "EyeLookOutRight", "EyeLookUpLeft", "EyeLookUpRight", "EyeSquintLeft", "EyeSquintRight", "EyeWideLeft", "EyeWideRight", "JawForward", "JawLeft", "JawOpen", "JawRight", "MouthClose", "MouthDimpleLeft", "MouthDimpleRight", "MouthFrownLeft", "MouthFrownRight", "MouthFunnel", "MouthLeft", "MouthLowerDownLeft", "MouthLowerDownRight", "MouthPressLeft", "MouthPressRight", "MouthPucker", "MouthRight", "MouthRollLower", "MouthRollUpper", "MouthShrugLower", "MouthShrugUpper", "MouthSmileLeft", "MouthSmileRight", "MouthStretchLeft", "MouthStretchRight", "MouthUpperUpLeft", "MouthUpperUpRight", "NoseSneerLeft", "NoseSneerRight", "TongueOut" };
        string[] Other_BlendShapes = new string[] { "v_aa", "v_e", "v_ee", "v_ih", "v_oh", "v_ou", "v_sil", "v_ch", "v_dd", "v_ff", "v_kk", "v_nn", "v_pp", "v_rr", "v_ss", "v_th", "LeftBlink", "RightBlink", "Blink", "aa", "e", "ee", "ih", "oh", "ou", "sil", "ch", "dd", "ff", "kk", "nn", "pp", "rr", "ss", "th" };
        string[] Other_BlendShapes_Names = new string[] { "A", "E", "E", "I", "O", "U", "SIL", "CH", "DD", "FF", "KK", "NN", "PP", "RR", "SS", "TH", "Blink_L", "Blink_R", "Blink", "A", "E", "E", "I", "O", "U", "SIL", "CH", "DD", "FF", "KK", "NN", "PP", "RR", "SS", "TH" };

        EditorGUILayout.LabelField("Auto setup vrm blendshapes", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("\n\n", EditorStyles.whiteLabel);
        SaveLocation = EditorGUILayout.TextField("Save Location", SaveLocation);
        if (GUILayout.Button("Set Save Location"))
        {
            SaveLocation = EditorUtility.OpenFolderPanel("Save Folder", "", "");
            AssetDatabase.Refresh();
        }

        EditorGUILayout.LabelField("\n\n", EditorStyles.whiteLabel);
        name = EditorGUILayout.TextField("Avatar Name", name);
        //EditorGUILayout.LabelField("\n", EditorStyles.boldLabel);

        Avatar = EditorGUILayout.ObjectField("Avatar prefab", Avatar, typeof(GameObject), true) as GameObject;

        EditorGUILayout.LabelField("\n\n", EditorStyles.boldLabel);

        if (GUILayout.Button("Setup Data!"))
        {
            Debug.Log("Starting vrm setup");

            var skinned_mesh = Avatar.GetComponent<SkinnedMeshRenderer>();

            var shared_mesh = skinned_mesh.sharedMesh;

            List<string> BlendShape = new List<string>();

            if(!Directory.Exists(SaveLocation + @"/Clips"))
            {
                Directory.CreateDirectory(SaveLocation + @"/Clips");
            }

            var arkit_lowwer = Array.ConvertAll(arkit_BlendShapes, d => d.ToLower());

            //Generate a neutural clip

            //Generate Clips arkit
            for (int i = 0; i < shared_mesh.blendShapeCount; i++)
            {
                int index = Array.IndexOf(arkit_lowwer, shared_mesh.GetBlendShapeName(i).Trim().ToLower());
                //look for arkit face tracking
                if (index != -1)
                {
                    BlendShape.Add(arkit_BlendShapes[index]);
                    var Clip = ScriptableObject.CreateInstance<BlendShapeClip>();
                    string path = "Assets/" + SaveLocation.Split(new[] { "Assets" }, StringSplitOptions.None)[1] + @"/Clips/" + arkit_BlendShapes[index] + ".asset";
                    //path = "Assets/test/Clips/" + shared_mesh.GetBlendShapeName(i).Trim() + ".asset";
                    Clip.BlendShapeName = arkit_BlendShapes[index];
                    var Data = new VRM.BlendShapeBinding();
                    Data.Weight = 100;
                    Data.RelativePath = "Body";
                    Data.Index = i;
                    var array = new VRM.BlendShapeBinding[1];
                    array[0] = Data;
                    Clip.Values = array;
                    AssetDatabase.CreateAsset(Clip, path);
                    AssetDatabase.ImportAsset(path);
                }
            }
            BlendShape.Sort();

            var other_lowwer = Array.ConvertAll(Other_BlendShapes, d => d.ToLower());
            //generate other clips
            for (int i = 0; i < shared_mesh.blendShapeCount; i++)
            {
                int index = Array.IndexOf(other_lowwer, shared_mesh.GetBlendShapeName(i).Trim().ToLower());
                //look for other blendshapes
                if (index != -1)
                {
                    BlendShape.Add(Other_BlendShapes_Names[index]);
                    var Clip = ScriptableObject.CreateInstance<BlendShapeClip>();
                    string path = "Assets/" + SaveLocation.Split(new[] { "Assets" }, StringSplitOptions.None)[1] + @"/Clips/" + Other_BlendShapes_Names[index] + ".asset";
                    //path = "Assets/test/Clips/" + shared_mesh.GetBlendShapeName(i).Trim() + ".asset";
                    Clip.BlendShapeName = Other_BlendShapes_Names[index];
                    var Data = new VRM.BlendShapeBinding();
                    Data.Weight = 100;
                    Data.RelativePath = "Body";
                    Data.Index = i;
                    var array = new VRM.BlendShapeBinding[1];
                    array[0] = Data;
                    Clip.Values = array;
                    AssetDatabase.CreateAsset(Clip, path);
                    AssetDatabase.ImportAsset(path);
                }
            }



            //Add clips to a Blend Shape Avatar
            List<string> guids = new List<string>();
            long id = 0;



            BlendShape.Insert(0, "Neutral");
            var Clip2 = ScriptableObject.CreateInstance<BlendShapeClip>();
            string path5 = "Assets/" + SaveLocation.Split(new[] { "Assets" }, StringSplitOptions.None)[1] + @"/Clips/" + "Neutral" + ".asset";
            //path = "Assets/test/Clips/" + shared_mesh.GetBlendShapeName(i).Trim() + ".asset";
            Clip2.BlendShapeName = "Neutral";
            AssetDatabase.CreateAsset(Clip2, path5);
            AssetDatabase.ImportAsset(path5);

            foreach (var obj in BlendShape)
            {
                string[] lines = System.IO.File.ReadAllLines(SaveLocation + @"/Clips/" + obj + ".asset.meta");
                guids.Add(lines[1].Split(' ')[1].Trim());
                id = Convert.ToInt64(lines[4].Split(':')[1].Trim());
            }

            var AvatarData = ScriptableObject.CreateInstance<BlendShapeAvatar>();
            string path3 = SaveLocation + @"/" + name + "_AvatarBlendShape.asset";
            string path2 = "Assets" + SaveLocation.Split(new[] { "Assets" }, StringSplitOptions.None)[1] + "/" + name + "_AvatarBlendShape.asset";

            AssetDatabase.CreateAsset(AvatarData, path2);
            AssetDatabase.ImportAsset(path2);

            StreamReader sr = new StreamReader(path3);

            string TmpData = sr.ReadToEnd();

            sr.Close();

            bool latch = true;
            foreach (var obj in guids)
            {
                if (latch)
                {
                    TmpData += "  - {fileID:" + id + ", guid: " + obj + ", type: 2}";
                }
                else
                {
                    TmpData += "\n  - {fileID:" + id + ", guid: " + obj + ", type: 2}";
                }
                latch = false;
            }


            TmpData = TmpData.Replace("  Clips: []", "  Clips:");
            StreamWriter streamWriter = new StreamWriter(path3);

            streamWriter.Write(TmpData);

            streamWriter.Close();

            AssetDatabase.Refresh();


            Debug.Log($"Sucessfully created: {BlendShape.Count} vrm keys for avatar: {name}");
        }

        if (Avatar != null)
        {
            try
            {
                var skinned_mesh = Avatar.GetComponent<SkinnedMeshRenderer>();

                var shared_mesh = skinned_mesh.sharedMesh;

                string tmpMsg = $"{shared_mesh.blendShapeCount} BlendShapes detected on avatar: \n\n";

                bool latch = true;

                for (int i = 0; i < shared_mesh.blendShapeCount; i++)
                {
                    if (latch)
                    {
                        tmpMsg += shared_mesh.GetBlendShapeName(i).Trim();
                    }
                    else
                    {
                        tmpMsg += "\n" + shared_mesh.GetBlendShapeName(i).Trim();
                    }

                    latch = false;
                }

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
                GUILayout.Label(tmpMsg);
                EditorGUILayout.EndScrollView();
            }
            catch
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
                GUILayout.Label("No valid blend shapes detected on current avatar");
                EditorGUILayout.EndScrollView();
            }
        }
        else
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            GUILayout.Label("No selected avatar");
            EditorGUILayout.EndScrollView();
        }


        this.Repaint();
    }
}

public class ClipValue
{
    public string RelativePath = "Body";
    public int Index;
    public float Weight;
}