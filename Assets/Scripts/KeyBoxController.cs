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
    /// キーボックスの処理
    /// @author h.adachi
    /// </summary>
    public class KeyBoxController : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            // プレイヤーがキーを持って接触したら
            this.OnCollisionEnterAsObservable()
                .Where(t => t.gameObject.name.Equals("Player"))
                .Subscribe(t => {
                    foreach (Transform _child in t.gameObject.transform) {
                        if (_child.name.Contains("Key")) {
                            var _gameSystem = GameObject.Find("GameSystem"); // レベルクリア
                            _gameSystem.GetComponent<GameSystem>().ClearLevel();
                        }
                    }
                });
        }
    }

}