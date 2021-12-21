// Blade™ Server X
// Copyright (C) 2021, 2022 BudPlaza project & contributors.
// Licensed under GNU AGPL v3 or any later version; see COPYING for information.

using BudPlaza.BladeX.UserData;
using BudPlaza.BladeX.UserData.Structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudPlaza.BladeX.Users
{
    internal static class Permission
    {
        private static readonly List<string> _operators = new List<string>();

        internal static bool IsOperator(string cfxId)
        {
            return _operators.Contains(cfxId);
        }

        internal static void AddNewOp(string cfxId)
        {
            _operators.Add(cfxId);

            var file = new OperatorsList
            {
                Version = 1,
                Operators = _operators.ToArray()
            };
            File.WriteAllText(UserDataUtil.OperatorsFile, JsonConvert.SerializeObject(file));
        }

        internal static void Initialize()
        {
            _operators.Clear();

            if (File.Exists(UserDataUtil.OperatorsFile))
            {
                var list = JsonConvert.DeserializeObject<OperatorsList>(File.ReadAllText(UserDataUtil.OperatorsFile));

                foreach (var cfxId in list.Operators)
                {
                    _operators.Add(cfxId);
                }
            }
            else
            {
                File.WriteAllText(UserDataUtil.OperatorsFile, JsonConvert.SerializeObject(new OperatorsList()));
            }
        }
    }
}
