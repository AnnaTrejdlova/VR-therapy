using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallManager: Singleton<WallManager> {
    /// <summary>
    /// managing wall-related resources
    /// </summary>
    public GameObject WallJointPrefab;
    public GameObject WallFillPrefab;
    public Material WallMaterial;

    protected override void Awake() {
        base.Awake();
    }

}
