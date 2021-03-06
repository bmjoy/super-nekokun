﻿/*
 * Copyright 2002-2020 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace StudioMeowToon {
    /// <summary>
    /// 砲台の処理
    /// @author h.adachi
    /// </summary>
    public class CannonController : MonoBehaviour {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // フィールド

        [SerializeField]
        private GameObject player; // プレイヤー

        [SerializeField]
        private GameObject bullet; // 弾の元

        [SerializeField]
        private float bulletSpeed = 2000f; // 弾の速度

        private SoundSystem soundSystem; // サウンドシステム

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // 更新メソッド

        // Awake is called when the script instance is being loaded.
        void Awake() {
            // SoundSystem 取得
            soundSystem = GameObject.Find("SoundSystem").GetComponent<SoundSystem>();
        }

        // Start is called before the first frame update.
        void Start() {
            // FixedUpdate is called just before each physics update.
            this.FixedUpdateAsObservable()
                .Subscribe(_ => {
                    if (transform.name.Contains("_Piece")) { return; } // '破片' は無視する
                    var _random = Mathf.FloorToInt(Random.Range(0.0f, 175.0f));
                    if (_random == 3.0f) { // 3の時だけ
                        float _SPEED = 50.0f; // 回転スピード
                        Vector3 _look = player.transform.position - transform.position; // ターゲット方向へのベクトル
                        Quaternion _rotation = Quaternion.LookRotation(new Vector3(_look.x, _look.y + 0.5f, _look.z)); // 回転情報に変換 // TODO: 距離が遠い時に
                        transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, _SPEED * Time.deltaTime); // 徐々に回転

                        // 弾の複製
                        var _bullet = Instantiate(bullet) as GameObject;

                        // 弾の位置
                        var _pos = transform.position + transform.forward * 1.7f; // 前進させないと弾のコライダーが自分に当たる
                        _bullet.transform.position = new Vector3(_pos.x, _pos.y + 0.5f, _pos.z); // 0.5f は Y軸を中心に合わせる為

                        // 弾の回転
                        _bullet.transform.rotation = transform.rotation;

                        // 弾へ加える力
                        var _force = transform.forward * bulletSpeed;

                        // 弾を発射
                        _bullet.GetComponent<Rigidbody>().AddForce(_force, ForceMode.Acceleration);
                        soundSystem.PlayShootClip();
                    }
                });
        }
    }

}
