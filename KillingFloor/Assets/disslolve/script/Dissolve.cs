using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyGameStudio.Jeremy
{
    public class Dissolve : MonoBehaviour
    {
        [Header("show speed")]
        [Range(0, 5f)]
        public float speed_show;

        [Header("hide speed")]
        [Range(0, 5f)]
        public float hide_show;

        private Material _material;
        private bool is_showing=false;
        private bool is_hiding=false;
        private float threshold = 0;


        [Header("min max threshold")]
        public float min_threshold;
        public float max_threshold;


        // Start is called before the first frame update
        void Start()
        {
            if (this.GetComponent<MeshRenderer>() == null)
            {
                this._material = this.GetComponent<SkinnedMeshRenderer>().material;
            }
            else
            {
                this._material = this.GetComponent<MeshRenderer>().material;
            }
        }

        public void reset_material()
        {
            if (this.GetComponent<MeshRenderer>() == null)
            {
                this._material = this.GetComponent<SkinnedMeshRenderer>().material;
            }
            else
            {
                this._material = this.GetComponent<MeshRenderer>().material;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (this.is_showing)
            {
                //this.threshold = Mathf.Lerp(this.threshold, this.min_threshold, Time.deltaTime * this.speed_show);

                this.threshold -= Time.deltaTime * this.speed_show;

                if (this.threshold <= this.min_threshold)
                {
                    this.threshold = this.min_threshold;

                    this.is_showing = false;
                }

                this._material.SetFloat("_threshold", this.threshold);
            }

            if (this.is_hiding)
            {
                //this.threshold = Mathf.Lerp(this.threshold, this.max_threshold, Time.deltaTime * this.speed_show);

                this.threshold += Time.deltaTime * this.speed_show;

                if (this.threshold >= this.max_threshold)
                {
                    this.threshold = this.max_threshold;

                    this.is_hiding = false;
                }
                this._material.SetFloat("_threshold", this.threshold);
            }
        }

        public void show()
        {
            this.is_hiding = false;

            this.threshold = this.max_threshold;

            this._material.SetFloat("_threshold", this.threshold);

            this.is_showing = true;
        }

        public void hide()
        {
            this.is_showing = false;

            this.threshold = this.min_threshold;

            this._material.SetFloat("_threshold", this.threshold);

            this.is_hiding = true;
        }
    }
}