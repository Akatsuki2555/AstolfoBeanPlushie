using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSCLoader;
using UnityEngine;

namespace AstolfoBeanPlushie
{
    public class AstolfoBeanPlushie : Mod
    {
        public override string ID => "AstolfoBeanPlushie";

        public override string Version => "0.1";

        public override string Author => "アカツキ";

        public override void ModSetup()
        {
            base.ModSetup();

            SetupFunction(Setup.OnLoad, Mod_Load);
            SetupFunction(Setup.OnSave, Mod_OnSave);
        }

        private List<GameObject> _plushies = new List<GameObject>();
        private GameObject _plushie;
        private GameObject _plushiePrefab;

        public override void ModSettings()
        {
            base.ModSettings();

            Settings.AddButton(this, "Spawn Plushie", () =>
            {
                var player = GameObject.Find("PLAYER");
                SpawnPlushie(player.transform.position, player.transform.rotation);
            });

            Settings.AddButton(this, "Spawn 5 Plushies", () =>
            {
                var player = GameObject.Find("PLAYER");
                for (int i = 0; i < 5; i++)
                {
                    SpawnPlushie(player.transform.position, player.transform.rotation);
                }
            });

            Settings.AddButton(this, "Spawn 10 Plushies", () =>
            {
                var player = GameObject.Find("PLAYER");
                for (int i = 0; i < 10; i++)
                {
                    SpawnPlushie(player.transform.position, player.transform.rotation);
                }
            });

            Settings.AddButton(this, "Delete All Plushies", () =>
            {
                foreach (var item in _plushies)
                {
                    GameObject.Destroy(item);
                }

                _plushies.Clear();
            });
        }

        private GameObject SpawnPlushie(Vector3 pos, Quaternion rot)
        {
            _plushie = GameObject.Instantiate(_plushiePrefab);
            _plushie.AddComponent<Rigidbody>();
            _plushie.MakePickable();
            _plushie.name = "Astolfo Plushie(Clone)";
            _plushie.transform.position = pos;
            _plushie.transform.rotation = rot;
            return _plushie;
        }

        private void Mod_Load()
        {
            var assetBundle = AssetBundle.CreateFromMemoryImmediate(Resource1.astolfoplushie);
            _plushiePrefab = assetBundle.LoadAsset<GameObject>("Astolfo");
            assetBundle.Unload(false);

            var player = GameObject.Find("PLAYER");

            if (SaveLoad.ValueExists(this, "plushies"))
            {
                var plushies = SaveLoad.DeserializeClass<SavedPlushies>(this, "plushies");
                foreach (var plushie in plushies.Plushies)
                {
                    SpawnPlushie(plushie.Position, plushie.Rotation);
                }
            }

            // Migrate from previous commit
            if (SaveLoad.ValueExists(this, "plushiePos") && SaveLoad.ValueExists(this, "plushieRot"))
            {
                var pos = SaveLoad.ReadValue<Vector3>(this, "plushiePos");
                var rot = SaveLoad.ReadValue<Quaternion>(this, "plushieRot");
                SpawnPlushie(pos, rot);

                SaveLoad.DeleteValue(this, "plushiePos");
                SaveLoad.DeleteValue(this, "plushieRot");
            }

        }

        private void Mod_OnSave()
        {
            var saved = new List<SavedPlushie>();
            foreach (var plushie in _plushies)
            {
                saved.Add(new SavedPlushie(plushie.transform.position, plushie.transform.rotation));
            }

            SaveLoad.SerializeClass<SavedPlushies>(this, new SavedPlushies(saved), "plushies");
        }
    }

    [Serializable]
    public class SavedPlushies
    {
        public List<SavedPlushie> Plushies;

        public SavedPlushies()
        {
            Plushies = new List<SavedPlushie>();
        }

        public SavedPlushies(List<SavedPlushie> toImport)
        {
            Plushies = new List<SavedPlushie>();
            Plushies.AddRange(toImport);
        }
    }

    [Serializable]
    public class SavedPlushie
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public SavedPlushie() { }
        public SavedPlushie(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
