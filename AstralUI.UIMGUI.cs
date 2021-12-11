using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Astrum.AstralCore.Managers.CommandManager;
using static Astrum.AstralCore.Managers.ModuleManager;

[assembly: MelonInfo(typeof(Astrum.AstralUI.UIMGUI), "AstralUI.UIMGUI", "0.2.0", downloadLink: "github.com/Astrum-Project/AstralUI.UIMGUI")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace Astrum.AstralUI
{
    public class UIMGUI : MelonMod
    {
        public static Color32 color = new(0x56, 0x00, 0xA5, 0xFF);
        public static Module currentModule = null;

        private static Rect position = new(Screen.width / 2 - 78, Screen.height / 2 - 307, 156, 482);

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
            int i = 0;

            foreach (KeyValuePair<string, Module> module in modules)
                if (GUI.Button(new Rect(3, i++ * 22 + 21, 150, 20), module.Key))
                    currentModule = module.Value;

            GUI.DragWindow(new Rect(0, 0, 156, 20));
        }

        private void CommandsWindow(int windowID)
        {
            int i = 0;

            foreach (KeyValuePair<string, Command> command in currentModule.commands)
            {
                if (command.Value is ConVar<bool> cb) 
                    cb.Value = GUI.Toggle(CreateRect(ref i), cb.Value, command.Key);
                else if (command.Value is ConVar<float> cf)
                {
                    GUI.Label(CreateRect(ref i), command.Key + $": ({cf.Value:0.00})");
                    cf.Value = GUI.HorizontalSlider(CreateRect(ref i), cf.Value, cf.Value - 10, cf.Value + 10);
                }
                else if (command.Value is Button)
                {
                    if (GUI.Button(CreateRect(ref i), command.Key))
                        (command.Value as Button)?.onClick();
                }
                else GUI.Label(CreateRect(ref i), command.Key + " (Unsupported type)");
            }
        }

        private static Rect CreateRect(ref int i) => new(3, i++ * 22 + 21, 300, 20);
    }
}
