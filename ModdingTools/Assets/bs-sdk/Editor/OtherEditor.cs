using UnityEditor;

[CustomPropertyDrawer(typeof(SubsetListTeamEnum))]
[CustomPropertyDrawer(typeof(SubsetListAttachments))]
[CustomPropertyDrawer(typeof(SubsetListWeaponType))]
public class SubsetListProperyDrawer2 : SubsetListProperyDrawer
{
}



[CustomPropertyDrawer(typeof(WalkSoundTags))]
[CustomPropertyDrawer(typeof(AnimationDict))]
[CustomPropertyDrawer(typeof(AnimationOffsetDict))]
//[CustomPropertyDrawer(typeof(GunDict))]
// [CustomPropertyDrawer(typeof(BodyDamageUI))]
[CustomPropertyDrawer(typeof(PlayerSkinDict))]
[CustomPropertyDrawer(typeof(PlayerClassDict))]
[CustomPropertyDrawer(typeof(BodyDamage))]
[CustomPropertyDrawer(typeof(BodyToTransform))]
[CustomPropertyDrawer(typeof(WeaponShopCollection))]
public class DictPropertyDrawer : SerializableDictionaryPropertyDrawer
{
}
