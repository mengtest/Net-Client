using Assets.scripts.GameLogic.Models;
using C2GNet;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.scripts.GameLogic.Managers
{
    public class CharacterManager
    {
        private static CharacterManager _instance;
        public static CharacterManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CharacterManager();

                }
                return _instance;
            }
        }

        public List<Character> characterList;
        public async  Task CreateCharacter() { 
             
        }
        /**
         * 添加角色
         * @param Creature
         */
        public void AddCharacter(Character character)
        {
            this.characterList.Add(character);
        }

        /**
         * 清空
         */
        public void Clear()
        {
            characterList=null;
        }

        /**
         * 创建角色
         */
        public async void CreateCharacter(NRoom nRoom)
        {
            //let teamArr = TeamType2.Blue == teamType2 ? BattleManager.Instance.MyTeam : BattleManager.Instance.EnemyTeam;
            //let roomUsers = TeamType2.Blue == teamType2 ? User.Instance.room.my : User.Instance.room.enemy;

            //for (let i = 0; i < teamArr.length; i++)
            //{
            //    let characterNode = teamArr[i] as Node;
            //    if (!roomUsers[i])
            //    {
            //        return;
            //    }
            //    let cId = roomUsers[i].cCharacterId;
            //    let characterDefine = DataManager.Instance.characters[cId];

            //    let resource = characterDefine.Resource;
            //    console.log('cId=' + cId + ',resource=' + resource)
            //   let prefab = await LoadResUtil.loadPrefab(resource);
            //    let node = instantiate(prefab) as Node;
            //    characterNode.addChild(node);

            //    //创建角色
            //    let character = new Creature(teamType2, node, characterDefine, roomUsers[i].user, CreatureType.Character);
            //    CreatureManager.Instance.AddCreature(node, character);
            //    CharacterManager.Instance.AddCharacter(character);
            //    //当前角色
            //    if (BattleGlobal.battleMode == BattleMode.Battle)
            //    {    //对局模式
            //        if (User.Instance.user.id == character.user.id)
            //        {
            //            BattleManager.Instance.currentCharacter = character;
            //        }
            //    }
            //    else if (BattleGlobal.battleMode == BattleMode.Live)
            //    {    //观看直播模式
            //        if (BattleGlobal.targetLiveUserId == character.user.id)
            //        {
            //            BattleManager.Instance.currentCharacter = character;
            //        }
            //    }
            //}
        }
    }
}
   