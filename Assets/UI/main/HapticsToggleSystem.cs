// using Unity.Entities;
// using UnityEngine;
//
// namespace UI.Main
// {
//     public partial class HapticsToggleSystem : SystemBase
//     {
//         protected override void OnCreate()
//         {
//             EntityManager.CreateEntity(typeof(HapticsToggle));
//         }
//
//         protected override void OnUpdate()
//         {
//             foreach (var hapticsToggle in SystemAPI.Query<RefRO<HapticsToggle>>().WithChangeFilter<HapticsToggle>())
//             {
//                 Debug.Log($"HapticsChanged:{hapticsToggle.ValueRO.Value}");
//             }
//         }
//
//         public bool ToggleHaptics()
//         {
//             var hapticsToggle = SystemAPI.GetSingletonRW<HapticsToggle>();
//             hapticsToggle.ValueRW.Value = !hapticsToggle.ValueRO.Value;
//             return hapticsToggle.ValueRO.Value;
//         }
//     }
// }