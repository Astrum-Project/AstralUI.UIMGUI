using Astrum.AstralCore.UI;
using Astrum.AstralCore.UI.Attributes;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

[assembly: MelonInfo(typeof(Astrum.AstralUI.UIMGUI), "AstralUI.UIMGUI", "1.1.0", downloadLink: "github.com/Astrum-Project/AstralUI.UIMGUI")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace Astrum.AstralUI
{
    public class UIMGUI : MelonMod
    {
        private static readonly UnhollowerBaseLib.Il2CppReferenceArray<GUILayoutOption> options = new(0);

        public static Color32 color = new(0x56, 0x00, 0xA5, 0xFF);
        public static Module currentModule = null;

        private static Rect position = new(Screen.width / 2 - 78, Screen.height / 2 - 307, 156, 482);
        private static Vector2 scrollModules = default;
        private static Vector2 scrollCommands = default;

        public override void OnApplicationStart()
        {
            MelonPreferences_Category category = MelonPreferences.CreateCategory("Astrum-AstralUI-UIMGUI", "AstralUI UIMGUI");

            category.CreateEntry(nameof(color), new Color32(0x56, 0x00, 0xA5, 0xFF), "Color");
        }

        public override void OnPreferencesSaved() => OnPreferencesLoaded();
        public override void OnPreferencesLoaded()
        {
            MelonPreferences_Category category = MelonPreferences.GetCategory("Astrum-AstralUI-UIMGUI");

            color = category.GetEntry<Color32>(nameof(color)).Value;
        }

        public override void OnGUI()
        {
            if (!Input.GetKey(KeyCode.Tab)) return;

            GUI.backgroundColor = color;
            GUI.color = Color.white;

            position = GUI.Window(25, position, (GUI.WindowFunction)ModulesWindow, "Modules");
            if (currentModule != null)
                GUI.Window(30, new Rect(position.left + 158, position.top, 306, 482), (GUI.WindowFunction)CommandsWindow, "Commands");
        }

        private void ModulesWindow(int id)
        {
            scrollModules = GUILayout.BeginScrollView(scrollModules, options);

            foreach (KeyValuePair<string, Module> module in CoreUI.Modules)
                if (GUILayout.Button(module.Key, options))
                    currentModule = module.Value;
            
            GUILayout.EndScrollView(true);

            GUI.DragWindow(new Rect(0, 0, 156, 20));
        }

        private void CommandsWindow(int windowID)
        {
            scrollCommands = GUILayout.BeginScrollView(scrollCommands, options);

            foreach (KeyValuePair<string, UIBase> command in currentModule.Commands)
            {
                if (command.Value is UIFieldProp<bool> cb) 
                    cb.Value = GUILayout.Toggle(cb.Value, command.Key, options);
                else if (command.Value is UIFieldProp<float> cf)
                {
                    GUILayout.Label(command.Key + $": ({cf.Value:0.00})", options);
                    cf.Value = GUILayout.HorizontalSlider(cf.Value, cf.Value - 10, cf.Value + 10, options);
                }
                else if (command.Value is UIButton)
                {
                    if (GUILayout.Button(command.Key, options))
                        (command.Value as UIButton)?.Click();
                }
                else GUILayout.Label(command.Key + " (Unsupported type)", options);
            }

            GUILayout.EndScrollView(true);
        }
    }
}
