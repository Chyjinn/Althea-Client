﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using RAGE;

namespace Client
{
    class CharacterEditor : Events.Script
    {
        public CharacterEditor()
        {
            Events.Add("ChangeSlider", ChangeFaceFeature);

        }

        public void ChangeFaceFeature(object[] args)
        {
            
        }



        
    }
}
