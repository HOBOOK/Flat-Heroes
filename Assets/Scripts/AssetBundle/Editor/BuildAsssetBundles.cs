using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildAsssetBundles : MonoBehaviour
{
    [MenuItem("Bundles/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        /*********************************************************************** 
         * * 이름 : BuildPipeLine.BuildAssetBundles() 
         * * 용도 : BuildPipeLine 클래스의 함수 BuildAssetBundles()는 에셋번들을 만들어줍니다. 
         * * 매개변수에는 String 값을 넘기게 되며, 빌드된 에셋 번들을 저장할 경로입니다. 
         * * 예를 들어 Assets 하위 폴더에 저장하려면 "Assets/AssetBundles"로 입력해야합니다. 
         * **********************************************************************/
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}
