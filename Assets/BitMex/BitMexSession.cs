﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.BitMex.Commands;
using Assets.CombinationKey;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.BitMex
{
    public class BitMexSession
    {
        public string ApiSecret;
        public string ApiKey;
        public string Email;
        public string ReferrerAccount;
        public string ReferrerEmail;
        public DateTime UpdateDateTime;
        public BitMexMacro Macro;

        public BitMexSession()
        {
            this.Macro = new BitMexMacro();
        }
    }
}
