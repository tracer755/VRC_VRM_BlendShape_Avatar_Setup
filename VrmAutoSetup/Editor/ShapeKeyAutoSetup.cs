using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRM;

public class ShapeKeyAutoSetup : EditorWindow
{
    //Made by Tracer755, Ritual neo was the one who requested this tools construction

    GameObject Avatar = null;
    string SaveLocation = "";
    string name = "";
    Vector2 scrollPos;

    string[] arkit_lowwer = new string[0];
    string[] other_lowwer = new string[0];

    //these are the blendshapes that the tool looks for
    string[] arkit_BlendShapes = new string[] { "BrowDownLeft", "BrowDownRight", "BrowInnerUp", "BrowOuterUpLeft", "BrowOuterUpRight", "CheekPuff", "CheekSquintLeft", "CheekSquintRight", "EyeBlinkLeft", "EyeBlinkRight", "EyeLookDownLeft", "EyeLookDownRight", "EyeLookInLeft", "EyeLookInRight", "EyeLookOutLeft", "EyeLookOutRight", "EyeLookUpLeft", "EyeLookUpRight", "EyeSquintLeft", "EyeSquintRight", "EyeWideLeft", "EyeWideRight", "JawForward", "JawLeft", "JawOpen", "JawRight", "MouthClose", "MouthDimpleLeft", "MouthDimpleRight", "MouthFrownLeft", "MouthFrownRight", "MouthFunnel", "MouthLeft", "MouthLowerDownLeft", "MouthLowerDownRight", "MouthPressLeft", "MouthPressRight", "MouthPucker", "MouthRight", "MouthRollLower", "MouthRollUpper", "MouthShrugLower", "MouthShrugUpper", "MouthSmileLeft", "MouthSmileRight", "MouthStretchLeft", "MouthStretchRight", "MouthUpperUpLeft", "MouthUpperUpRight", "NoseSneerLeft", "NoseSneerRight", "TongueOut" };
    string[] Other_BlendShapes = new string[] { "v_aa", "v_e", "v_ee", "v_ih", "v_oh", "v_ou", "v_sil", "v_ch", "v_dd", "v_ff", "v_kk", "v_nn", "v_pp", "v_rr", "v_ss", "v_th", "LeftBlink", "RightBlink", "Blink", "vrc.v_aa", "vrc.v_e", "vrc.v_ee", "vrc.v_ih", "vrc.v_oh", "vrc.v_ou", "vrc.v_sil", "vrc.v_ch", "vrc.v_dd", "vrc.v_ff", "vrc.v_kk", "vrc.v_nn", "vrc.v_pp", "vrc.v_rr", "vrc.v_ss", "vrc.v_th", "aa", "e", "ee", "ih", "oh", "ou", "sil", "ch", "dd", "ff", "kk", "nn", "pp", "rr", "ss", "th" };
    string[] Other_BlendShapes_Names = new string[] { "A", "E", "E", "I", "O", "U", "SIL", "CH", "DD", "FF", "KK", "NN", "PP", "RR", "SS", "TH", "Blink_L", "Blink_R", "Blink", "A", "E", "E", "I", "O", "U", "SIL", "CH", "DD", "FF", "KK", "NN", "PP", "RR", "SS", "TH", "A", "E", "E", "I", "O", "U", "SIL", "CH", "DD", "FF", "KK", "NN", "PP", "RR", "SS", "TH" };

    [MenuItem("Tools/Vrm/Auto setup shapekeys")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ShapeKeyAutoSetup));
    }


    private async void OnGUI()
    {
        if (arkit_BlendShapes.Length == 0 || other_lowwer.Length == 0)
        {
            arkit_lowwer = Array.ConvertAll(arkit_BlendShapes, d => d.ToLower());
            other_lowwer = Array.ConvertAll(Other_BlendShapes, d => d.ToLower());
        }


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

        //i hate that i have to wrap this in a try catch so unity dosent spaz when you open the object selector
        try
        {
            Avatar = EditorGUILayout.ObjectField("Avatar prefab", Avatar, typeof(GameObject), true) as GameObject;
        }
        catch (UnityEngine.ExitGUIException)
        {
            //throw;
        }
        catch (System.Exception e)
        {
            throw (e);
        }

        EditorGUILayout.LabelField("\n\n", EditorStyles.boldLabel);

        bool buttonEnabled = true;

        string tempBtnMsg = "";

        if (SaveLocation == "")
        {
            tempBtnMsg += "\nPlease add a save location";
            buttonEnabled = false;
        }
        else
        {
            try
            {
                string path = "Assets/" + SaveLocation.Split(new[] { "Assets" }, StringSplitOptions.None)[1] + @"/Clips/test.asset";
            }
            catch
            {
                buttonEnabled = false;
                tempBtnMsg += "\nSave location must be in your projects assets folder";
            }
        }
        if (name == "")
        {
            tempBtnMsg += "\nPlease give the avatar a name";
            buttonEnabled = false;
        }
        if (Avatar == null)
        {
            tempBtnMsg += "\nPlease input an avatar";
            buttonEnabled = false;
        }

        GUILayout.Label(tempBtnMsg.Trim());
        GUI.enabled = buttonEnabled;
        var goBtn = GUILayout.Button("Setup Data!");

        GUI.enabled = true;
        if (goBtn)
        {
            Debug.Log("Starting vrm setup");

            var skinned_mesh = Avatar.GetComponent<SkinnedMeshRenderer>();
            var shared_mesh = skinned_mesh.sharedMesh;

            List<string> BlendShape = new List<string>();

            if (!Directory.Exists(SaveLocation + @"/Clips"))
            {
                Directory.CreateDirectory(SaveLocation + @"/Clips");
            }

            //Generate Clips
            for (int i = 0; i < shared_mesh.blendShapeCount; i++)
            {
                string shape = shared_mesh.GetBlendShapeName(i).Trim().ToLower();
                int index = Array.IndexOf(arkit_lowwer, shape);
                //look for arkit face tracking
                if (index != -1)
                {
                    if (Array.IndexOf(BlendShape.ToArray(), arkit_BlendShapes[index]) != -1)
                    {
                        Debug.Log($"VRM Blendshape {BlendShape[Array.IndexOf(BlendShape.ToArray(), arkit_BlendShapes[index])]} has multiple valid blendshapes, Avatar Blendshape {arkit_BlendShapes[index]} will not be used for this VRM shape");
                    }
                    else
                    {
                        BlendShape.Add(arkit_BlendShapes[index]);
                        var Clip = ScriptableObject.CreateInstance<BlendShapeClip>();
                        string path = "Assets/" + SaveLocation.Split(new[] { "Assets" }, StringSplitOptions.None)[1] + @"/Clips/" + arkit_BlendShapes[index] + ".asset";
                        Clip.BlendShapeName = arkit_BlendShapes[index];
                        var Data = new VRM.BlendShapeBinding();
                        Data.Weight = 100;
                        //Data.RelativePath = "Body";
                        Data.RelativePath = Avatar.name;
                        Data.Index = i;
                        var array = new VRM.BlendShapeBinding[1];
                        array[0] = Data;
                        Clip.Values = array;
                        AssetDatabase.CreateAsset(Clip, path);
                    }
                }
                else
                {
                    int index2 = Array.IndexOf(other_lowwer, shape);
                    //look for other blendshapes
                    if (index2 != -1)
                    {
                        if (Array.IndexOf(BlendShape.ToArray(), Other_BlendShapes_Names[index2]) != -1)
                        {
                            Debug.Log($"VRM Blendshape {BlendShape[Array.IndexOf(BlendShape.ToArray(), Other_BlendShapes_Names[index2])]} has multiple valid blendshapes, Avatar Blendshape {Other_BlendShapes_Names[index2]} will not be used for this VRM shape");
                        }
                        else
                        {
                            BlendShape.Add(Other_BlendShapes_Names[index2]);
                            var Clip = ScriptableObject.CreateInstance<BlendShapeClip>();
                            string path = "Assets/" + SaveLocation.Split(new[] { "Assets" }, StringSplitOptions.None)[1] + @"/Clips/" + Other_BlendShapes_Names[index2] + ".asset";
                            Clip.BlendShapeName = Other_BlendShapes_Names[index2];
                            var Data = new VRM.BlendShapeBinding();
                            Data.Weight = 100;
                            //Data.RelativePath = "Body";
                            Data.RelativePath = Avatar.name;
                            Data.Index = i;
                            var array = new VRM.BlendShapeBinding[1];
                            array[0] = Data;
                            Clip.Values = array;
                            AssetDatabase.CreateAsset(Clip, path);
                        }
                    }
                }
            }
            //generate the avatar blendshape file before the assetdatabase refresh
            var AvatarData = ScriptableObject.CreateInstance<BlendShapeAvatar>();
            AssetDatabase.CreateAsset(AvatarData, "Assets" + SaveLocation.Split(new[] { "Assets" }, StringSplitOptions.None)[1] + "/" + name + "_AvatarBlendShape.asset");
            Debug.Log("Assets" + SaveLocation.Split(new[] { "Assets" }, StringSplitOptions.None)[1] + "/" + name + "_AvatarBlendShape.asset");
            BlendShape.Sort();

            //generate a neutral clip
            BlendShape.Insert(0, "Neutral");
            var Clip2 = ScriptableObject.CreateInstance<BlendShapeClip>();
            Clip2.BlendShapeName = "Neutral";
            AssetDatabase.CreateAsset(Clip2, "Assets/" + SaveLocation.Split(new[] { "Assets" }, StringSplitOptions.None)[1] + @"/Clips/" + "Neutral" + ".asset");
            AssetDatabase.Refresh();

            //Add clips to a Blend Shape Avatar
            List<string> guids = new List<string>();
            //long id = 0;
            string path3 = SaveLocation + @"/" + name + "_AvatarBlendShape.asset";
            StreamReader sr = new StreamReader(path3);
            string TmpData = sr.ReadToEnd();
            sr.Close();
            bool latch = true;
            foreach (var obj in BlendShape)
            {
                string[] lines = System.IO.File.ReadAllLines(SaveLocation + @"/Clips/" + obj + ".asset.meta");
                /*if (id == 0)
                {
                    id = Convert.ToInt64(lines[4].Split(':')[1].Trim());
                }*/

                if (latch)
                {
                    TmpData += "  - {fileID:" + Convert.ToInt64(lines[4].Split(':')[1].Trim()) + ", guid: " + lines[1].Split(' ')[1].Trim() + ", type: 2}";
                }
                else
                {
                    TmpData += "\n  - {fileID:" + Convert.ToInt64(lines[4].Split(':')[1].Trim()) + ", guid: " + lines[1].Split(' ')[1].Trim() + ", type: 2}";
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

                int count = 0;
                string tmpMsg = "";
                bool latch = true;

                for (int i = 0; i < shared_mesh.blendShapeCount; i++)
                {
                    if (Array.IndexOf(other_lowwer, shared_mesh.GetBlendShapeName(i).Trim().ToLower()) != -1 || Array.IndexOf(arkit_lowwer, shared_mesh.GetBlendShapeName(i).Trim().ToLower()) != -1)
                    {
                        count++;
                    }

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
                GUILayout.Label($"{shared_mesh.blendShapeCount} BlendShapes detected on avatar \n{count} are valid VRM shapes\n\n" + tmpMsg);
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

        GUILayout.Label("\n");
        if (GUILayout.Button("Tutorial"))
        {
            Application.OpenURL("https://www.youtube.com/watch?v=6omhxjDSFdE");
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