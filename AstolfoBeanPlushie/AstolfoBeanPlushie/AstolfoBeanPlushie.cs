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

        private GameObject _plushie;

        private void Mod_Load()
        {
            var assetBundle = AssetBundle.CreateFromMemoryImmediate(Resource1.astolfoplushie);

            var beanPlushie = assetBundle.LoadAsset<GameObject>("Astolfo");
            _plushie = GameObject.Instantiate(beanPlushie);
            _plushie.AddComponent<Rigidbody>();
            _plushie.MakePickable();
            _plushie.name = "Astolfo Plushie(Clone)";
            var player = GameObject.Find("PLAYER");
            _plushie.transform.position = player.transform.position;

            if (SaveLoad.ValueExists(this, "plushiePos") && SaveLoad.ValueExists(this, "plushieRot"))
            {
                _plushie.transform.position = SaveLoad.ReadValue<Vector3>(this, "plushiePos");
                _plushie.transform.rotation = SaveLoad.ReadValue<Quaternion>(this, "plushieRot");
            }

            assetBundle.Unload(false);
        }

        private void Mod_OnSave()
        {
            SaveLoad.WriteValue<Vector3>(this, "plushiePos", _plushie.transform.position);
            SaveLoad.WriteValue<Quaternion>(this, "plushieRot", _plushie.transform.rotation);
        }
    }
}
