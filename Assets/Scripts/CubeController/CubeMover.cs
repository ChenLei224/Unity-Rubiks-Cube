using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    public float speed;
    public Transform centerCube;
    public Transform rootCube;

    Vector3 orientation;
    int target_dim;
    float target_angle;

    Transform root;
    bool shouldDestroy;
    // Start is called before the first frame update
    void Start()
    {
        root = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (root != null) {
            if (shouldDestroy) {
                if (root.childCount>0) {
                    for (int i = 0; i < root.childCount; i++) {
                        root.GetChild(i).SetParent(rootCube);
                    }
                } else {
                    Destroy(root.gameObject);
                    root = null;
                    shouldDestroy = false;
                }
            } else {
                root.Rotate(orientation);
                if (target_dim == 0) {
                    if (target_angle > 0 && normedAngle(root.eulerAngles.x) >= target_angle) {
                        root.eulerAngles = new Vector3(target_angle, root.eulerAngles.y, root.eulerAngles.z);
                        cleanRoot();
                    }
                    else if (target_angle < 0 && normedAngle(root.eulerAngles.x) <= target_angle) {
                        root.eulerAngles = new Vector3(target_angle, root.eulerAngles.y, root.eulerAngles.z);
                        cleanRoot();
                    }

                }
                else if (target_dim == 1) {
                    if (target_angle > 0 && normedAngle(root.eulerAngles.y) >= target_angle) {
                        root.eulerAngles = new Vector3(root.eulerAngles.x, target_angle, root.eulerAngles.z);
                        cleanRoot();
                    }
                    else if (target_angle < 0 && normedAngle(root.eulerAngles.y) <= target_angle) {
                        root.eulerAngles = new Vector3(root.eulerAngles.x, target_angle, root.eulerAngles.z);
                        cleanRoot();
                    }

                }
                else if (target_dim == 2) {
                    if (target_angle > 0 && normedAngle(root.eulerAngles.z) >= target_angle) {
                        root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y, target_angle);
                        cleanRoot();
                    }
                    else if (target_angle < 0 && normedAngle(root.eulerAngles.z) <= target_angle) {
                        root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y, target_angle);
                        cleanRoot();
                    }
                }
            }
        }
    }

    float normedAngle(float x) {
        while (x>180) {
            x = x - 360;
        }
        while (x<=-180) {
            x = x + 360;
        }
        return x;
    }

    void moveCubes(Vector3 axis, bool is90Degree, bool isAll, int _orientation, int _target_dim, float _target_angle) {
        if (isAvailable()) {
            List<Transform> ts = findCubesInFront(axis, is90Degree, isAll);
            GameObject emptyGO = new GameObject();
            root = emptyGO.transform;
            foreach (Transform t in ts) {
                t.SetParent(root);
            }
            orientation = axis * _orientation * speed;
            target_dim = _target_dim;
            target_angle = _target_angle;
        }
    }

    void cleanRoot() {
        if (root!=null) {
            shouldDestroy = true;
        }
    }

    List<Transform> findCubesInFront(Vector3 axis, bool is90Degree, bool isAll) {
        List<Transform> result = new List<Transform>();
        if (isAll) {
            for (int i = 0; i < rootCube.childCount; i++) {
                Transform t = rootCube.GetChild(i);
                Vector3 v = t.position - centerCube.position;
                if (v.magnitude > 1e-4) {
                    result.Add(t);
                }
            }
        } else {
            for (int i = 0; i < rootCube.childCount; i++) {
                Transform t = rootCube.GetChild(i);
                Vector3 v = t.position - centerCube.position;
                if (v.magnitude > 1e-4) {
                    float cosine = Vector3.Dot(v, axis) / (v.magnitude * axis.magnitude);
                    if (is90Degree) cosine = Mathf.Abs(cosine);
                    if ((!is90Degree) && (cosine > 1e-4)) {
                        result.Add(t);
                    }
                    else if (is90Degree && (cosine < 1e-4)) {
                        result.Add(t);
                    }
                }
            }
        }
        return result;
    }

    public bool isAvailable() {
        if (root == null && shouldDestroy == false) {
            return true;
        } else {
            return false;
        }
    }

    public void move(string code) {
        switch (code) {
            case "A_FL":
                moveCubes(rootCube.forward, false, true, 1, 2, 90);
                break;
            case "A_FR":
                moveCubes(rootCube.forward, false, true, -1, 2, -90);
                break;
            case "A_RF":
                moveCubes(rootCube.right, false, true, -1, 0, -90);
                break;
            case "A_RB":
                moveCubes(rootCube.right, false, true, 1, 0, 90);
                break;
            case "A_UR":
                moveCubes(rootCube.up, false, true, -1, 1, -90);
                break;
            case "A_UL":
                moveCubes(rootCube.up, false, true, 1, 1, 90);
                break;
            case "F_L":
                moveCubes(-rootCube.forward, false, false, -1, 2, 90);
                break;
            case "F_R":
                moveCubes(-rootCube.forward, false, false, 1, 2, -90);
                break;
            case "Fm_L":
                moveCubes(-rootCube.forward, true, false, -1, 2, 90);
                break;
            case "Fm_R":
                moveCubes(-rootCube.forward, true, false, 1, 2, -90);
                break;
            case "B_L":
                moveCubes(rootCube.forward, false, false, 1, 2, 90);
                break;
            case "B_R":
                moveCubes(rootCube.forward, false, false, -1, 2, -90);
                break;
            case "R_F":
                moveCubes(rootCube.right, false, false, -1, 0, -90);
                break;
            case "R_B":
                moveCubes(rootCube.right, false, false, 1, 0, 90);
                break;
            case "Rm_F":
                moveCubes(rootCube.right, true, false, -1, 0, -90);
                break;
            case "Rm_B":
                moveCubes(rootCube.right, true, false, 1, 0, 90);
                break;
            case "L_F":
                moveCubes(-rootCube.right, false, false, 1, 0, -90);
                break;
            case "L_B":
                moveCubes(-rootCube.right, false, false, -1, 0, 90);
                break;
            case "U_R":
                moveCubes(rootCube.up, false, false, -1, 1, -90);
                break;
            case "U_L":
                moveCubes(rootCube.up, false, false, 1, 1, 90);
                break;
            case "Um_R":
                moveCubes(rootCube.up, true, false, -1, 1, -90);
                break;
            case "Um_L":
                moveCubes(rootCube.up, true, false, 1, 1, 90);
                break;
            case "D_R":
                moveCubes(-rootCube.up, false, false, 1, 1, -90);
                break;
            case "D_L":
                moveCubes(-rootCube.up, false, false, -1, 1, 90);
                break;
        }
    }
}
