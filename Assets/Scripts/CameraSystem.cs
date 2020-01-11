﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace StudioMeowToon {
    /// <summary>
    /// カメラ関連の処理
    /// </summary>
    public class CameraSystem : MonoBehaviour {

        // TODO: カメラの視野を調節する機能：ステップ式で良い

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // カメラ用 GameObject への参照

        [SerializeField]
        private GameObject mainCamera;

        [SerializeField]
        private GameObject verticalAxis;

        [SerializeField]
        private GameObject horizontalAxis;

        // 光源:デフォルト 44,16,100,100
        
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // フィールド

        private Vector3 defaultLocalPosition; // カメラシステム位置 デフォルト値

        private bool isForwardPosition = false; // カメラシステムが前進ポジションかどうか

        //////////////////////////////////////////////////////
        // ゲームパッド ボタン

        private ButtonControl aButton;

        private ButtonControl bButton;

        private ButtonControl xButton;

        private ButtonControl yButton;

        private ButtonControl dpadUp;

        private ButtonControl dpadDown;

        private ButtonControl dpadLeft;

        private ButtonControl dpadRight;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // パブリックメソッド

        public void LookPlayer() { // TODO: 再検討
            transform.localPosition = new Vector3(0, 0.75f, 0.8f);
            transform.localRotation = new Quaternion(0, -180, 0, 0);
        }

        public void ResetLookAround() { // カメラリセット
            resetLookAround();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // 更新メソッド

        // Start is called before the first frame update
        void Start() {
            defaultLocalPosition = transform.localPosition; // デフォルトのカメラポジション保存
        }

        // Update is called once per frame
        void Update() {
            // OS判定とゲームパッドのキー参照
            dpadUp = Gamepad.current.dpad.up;
            dpadDown = Gamepad.current.dpad.down;
            dpadLeft = Gamepad.current.dpad.left;
            dpadRight = Gamepad.current.dpad.right;
            if (Application.platform == RuntimePlatform.Android) {
                // Android
                aButton = Gamepad.current.aButton;
                bButton = Gamepad.current.bButton;
                xButton = Gamepad.current.xButton;
                yButton = Gamepad.current.yButton;
            } else if (Application.platform == RuntimePlatform.WindowsPlayer) {
                // Windows
                aButton = Gamepad.current.bButton;
                bButton = Gamepad.current.aButton;
                xButton = Gamepad.current.yButton;
                yButton = Gamepad.current.xButton;
            } else {
                // Unityで開発中は取れない？
                aButton = Gamepad.current.bButton;
                bButton = Gamepad.current.aButton;
                xButton = Gamepad.current.yButton;
                yButton = Gamepad.current.xButton;
            }

            // 視点操作のリセット
            if (xButton.wasReleasedThisFrame) {
                resetLookAround();
                return;
            }

            // 視点操作(Xボタン押しっぱなし)
            if (xButton.isPressed) {
                lookAround();
                return;
            }
        }

        // LateUpdate is called after all Update functions have been called.
        void LateUpdate() {
            if (!xButton.isPressed) { // 視点操作モードでない場合
                float _ADJUST = 80.0f; // 移動係数
                if (isForwardPosition) {
                    if (transform.localPosition.z < -0.5) { // カメラシステムをプレイヤーの頭のすぐ後ろまで移動する
                        transform.localPosition = new Vector3(
                            transform.localPosition.x,
                            transform.localPosition.y,
                            transform.localPosition.z + 0.075f * Time.deltaTime * _ADJUST
                        );
                    }
                    transform.localRotation = new Quaternion(0, 0, 0, 0);
                    checkSix();
                } else if (!isForwardPosition) {
                    if (transform.localPosition.z > defaultLocalPosition.z) {
                        transform.localPosition = new Vector3(
                            transform.localPosition.x,
                            transform.localPosition.y,
                            transform.localPosition.z - 0.035f * Time.deltaTime * _ADJUST
                        );
                    }
                    if (transform.localPosition.y > defaultLocalPosition.y) {
                        transform.localPosition = new Vector3(
                            transform.localPosition.x,
                            transform.localPosition.y - 0.035f * Time.deltaTime * _ADJUST,
                            transform.localPosition.z
                        );
                    }
                    transform.localRotation = new Quaternion(0, 0, 0, 0);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // イベントハンドラ

        void OnTriggerEnter(Collider other) {
            if (other.name.Contains("Block") || other.name.Contains("Wall")) {
                isForwardPosition = true; // カメラ前進ポジションフラグON
            }
        }

        void OnTriggerStay(Collider other) {
            if (other.name.Contains("Block") || other.name.Contains("Wall")) {
                isForwardPosition = true; // カメラ前進ポジションフラグON
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // プライベートメソッド

        private void checkSix() {
            var _player = transform.parent.gameObject;
            Ray _ray = new Ray( // プレイヤーの位置から後方を確認する
                new Vector3(
                    _player.transform.position.x,
                    _player.transform.position.y + 0.6f, // カメラシステムのコライダーと同じ高さにする
                    _player.transform.position.z
                ),
                -transform.forward
            );
            if (Physics.Raycast(_ray, out RaycastHit _hit, 1.0f)) {
#if DEBUG
                Debug.DrawRay(_ray.origin, _ray.direction * 1.0f, Color.magenta, 3, false); //レイを可視化
#endif
            } else { // 後方にレイを投げて反応がなくなった場合
                isForwardPosition = false; // 前進ポジションフラグOFF
            }
        }

        private void resetLookAround() { // 一人称視点カメラリセット
            // 位置 X: 0, Y:0.95, Z:-1.2 ※CameraSystemに設定 // Y:0.75 → 0.95
            // 回転 X:15, Y:0   , Z:0    ※MainCameraに設定
            // 拡縮 X: 1, Y:1   , Z:1
            transform.localPosition = defaultLocalPosition; // カメラシステム位置リセット
            transform.localRotation = new Quaternion(0, 0, 0, 0);
            horizontalAxis.transform.localRotation = new Quaternion(0, 0, 0, 0); // カメラ水平回転リセット
            verticalAxis.transform.localRotation = new Quaternion(0, 0, 0, 0); // カメラ垂直回転リセット
            mainCamera.transform.localEulerAngles = new Vector3(15f, 0f, 0f); // カメラ本体は少し下向き
        }

        private void lookAround() { // 一人称視点カメラ操作
            float _ADJUST = 80.0f; // 回転係数
            mainCamera.transform.localEulerAngles = new Vector3(0f, 0f, 0f); // カメラ本体を水平に
            if (dpadUp.isPressed) { // 上
                verticalAxis.transform.Rotate(1.0f * Time.deltaTime * _ADJUST, 0f, 0f);
            } else if (dpadDown.isPressed) { // 下
                verticalAxis.transform.Rotate(-1.0f * Time.deltaTime * _ADJUST, 0f, 0f);
            } else if (dpadLeft.isPressed) { // 左
                horizontalAxis.transform.Rotate(0f, -1.0f * Time.deltaTime * _ADJUST, 0f);
            } else if (dpadRight.isPressed) { // 右
                horizontalAxis.transform.Rotate(0f, 1.0f * Time.deltaTime * _ADJUST, 0f);
            }
            if (transform.localPosition.z < 0.1f) { // カメラシステムをキャラの目の位置に移動する
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    transform.localPosition.y,
                    transform.localPosition.z + 0.075f * Time.deltaTime * _ADJUST
                );
            }
        }
    }

}