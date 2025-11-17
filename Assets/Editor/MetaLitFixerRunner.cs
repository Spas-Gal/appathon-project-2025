using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Lightweight runner for the Meta/Lit fixer so it can be invoked from -executeMethod.
public static class MetaLitFixerRunner
{
    // Find materials referencing Meta/Lit and log them (Editor Console).
    // Run from Editor: Tools → Meta → Find Meta/Lit Materials
    // Or from command-line: Unity -batchmode -projectPath <path> -executeMethod MetaLitFixerRunner.RunFind -quit
    public static void RunFind()
    {
        try
        {
            var found = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null) continue;
                if (mat.shader != null && mat.shader.name != null && mat.shader.name.StartsWith("Meta/Lit"))
                {
                    found.Add(path);
                    Debug.Log($"Found Meta/Lit material: {path}", mat);
                }
            }

            Debug.Log($"MetaLitFinder: Found {found.Count} Meta/Lit material(s) in Assets/.");

            // In interactive editor show a dialog; in batchmode this does nothing.
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Meta/Lit Finder", $"Found {found.Count} material(s) referencing Meta/Lit in Assets/. See Console for list.", "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("MetaLitFinder.RunFind failed: " + ex);
            throw;
        }
    }

    // Replace Meta/Lit with Universal Render Pipeline/Lit for materials under Assets/ with a backup.
    // Run from Editor menu or from command-line: Unity -batchmode -projectPath <path> -executeMethod MetaLitFixerRunner.RunReplace -quit
    public static void RunReplace()
    {
        try
        {
            string backupFolder = $"Assets/MaterialBackups/MetaLitBackup_{DateTime.Now:yyyyMMdd_HHmmss}";
            if (!AssetDatabase.IsValidFolder("Assets/MaterialBackups"))
            {
                AssetDatabase.CreateFolder("Assets", "MaterialBackups");
            }
            AssetDatabase.CreateFolder("Assets/MaterialBackups", $"MetaLitBackup_{DateTime.Now:yyyyMMdd_HHmmss}");

            var changed = new List<string>();
            var skipped = new List<string>();

            string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null) continue;
                if (mat.shader != null && mat.shader.name != null && mat.shader.name.StartsWith("Meta/Lit"))
                {
                    // backup
                    string fileName = Path.GetFileName(path);
                    string backupPath = Path.Combine(backupFolder, fileName).Replace("\\", "/");
                    AssetDatabase.CopyAsset(path, backupPath);

                    Shader urp = Shader.Find("Universal Render Pipeline/Lit");
                    if (urp == null)
                    {
                        Debug.LogWarning("URP Lit shader not found in project. Skipping replacement for: " + path);
                        skipped.Add(path);
                        continue;
                    }

                    Undo.RecordObject(mat, "Replace Meta/Lit shader");
                    mat.shader = urp;
                    EditorUtility.SetDirty(mat);
                    changed.Add(path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Meta/Lit replacement finished. Replaced: {changed.Count}. Skipped: {skipped.Count}.");

            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Meta/Lit Replacer", $"Replaced: {changed.Count}. Skipped: {skipped.Count}. Backups stored in {backupFolder}", "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("MetaLitFinder.RunReplace failed: " + ex);
            throw;
        }
    }
}
