using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhAPI;

namespace RubezhClient
{
    public partial class ClientManager
    {
        public static void InvalidatePlans()
        {
        }
        public static bool PlanValidator(PlansConfiguration configuration)
        {
            return true;
        }
    }
}