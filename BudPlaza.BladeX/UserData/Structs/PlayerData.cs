// Blade™ Server X
// Copyright (C) 2021, 2022 BudPlaza project & contributors.
// Licensed under GNU AGPL v3 or any later version; see COPYING for information.

using CitizenFX.Core.Native;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudPlaza.BladeX.UserData.Structs
{
    public class PlayerData
    {
        public ObjectId Id { get; set; }
        public string Account { get; set; }
        public string UserName { get; set; }
        public bool HasVessel { get; set; }
        public Vessel? Vessel { get; set; }
    }

    public struct HeadOverlay
    {
        public int Value { get; set; }
        public int ColorType { get; set; }
        public int Color { get; set; }
        public int MixColor { get; set; }
    }

    public struct Vessel
    {
        public IDictionary<int, float> FaceFeatures { get; set; }
        public IDictionary<int, int> HeadOverlays { get; set; }
        public int ShapeFather { get; set; }
        public int ShapeMother { get; set; }
        public int ShapeAffector { get; set; }
        public int SkinFather { get; set; }
        public int SkinMother { get; set; }
        public int SkinAffector { get; set; }
        public float ShapeMix { get; set; }
        public float SkinMix { get; set; }
        public float ExtraMix { get; set; }
        public bool Parent { get; set; }
    }
}
