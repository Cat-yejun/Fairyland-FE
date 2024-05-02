using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Suriyun
{
    public class AnimatorController : MonoBehaviour
    {

        public Animator[] animators;
        public Animator animator;
        private int currentAnimationIndex = 0;
        const int MOVES = 2; 

        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                ChangeAnimation();
            }
        }

        void ChangeAnimation()
        {
            currentAnimationIndex++;
            if(currentAnimationIndex >= MOVES)
            {
                currentAnimationIndex = 0;
            }

            foreach(Animator anim in animators)
            {
                anim.SetInteger("NextInt", currentAnimationIndex);
            }
        }

        public void SwapVisibility(GameObject obj)
        {
            obj.SetActive(!obj.activeSelf);
        }


        public void SetFloat(string parameter = "key,value")
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);

            string name = param[0];
            float value = (float)Convert.ToDouble(param[1]);

            Debug.Log(name + " " + value);

            foreach (Animator a in animators)
            {
                a.SetFloat(name, value);
            }
        }
        public void SetInt(string parameter = "key,value")
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);

            string name = param[0];
            int value = Convert.ToInt32(param[1]);

            Debug.Log(name + " " + value);

            foreach (Animator a in animators)
            {
                a.SetInteger(name, value);
            }
        }

        public void SetBool(string parameter = "key,value")
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);

            string name = param[0];
            bool value = Convert.ToBoolean(param[1]);

            Debug.Log(name + " " + value);

            foreach (Animator a in animators)
            {
                a.SetBool(name, value);
            }
        }

        public void SetTrigger(string parameter = "key,value")
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);

            string name = param[0];

            Debug.Log(name);

            foreach (Animator a in animators)
            {
                a.SetTrigger(name);
            }
        }
    }
}