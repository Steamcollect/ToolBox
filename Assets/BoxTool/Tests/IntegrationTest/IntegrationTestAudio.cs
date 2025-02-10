using System.Collections;
using System.Collections.Generic;
using BT.Audio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IntegrationTestAudio
{
    [UnityTest]
    public IEnumerator Should_audio_manager_create_sound_container_when_start()
    {
        AudioManager audioManager = Object.Instantiate(TestUtils.LoadAssetEditorMode<GameObject>("AudioManager")).GetComponent<AudioManager>();
        yield return null;
        Assert.AreEqual(audioManager.GetFieldValue<int>("startingAudioObjectsCount"),audioManager.GetFieldValue<Queue<AudioSource>>("_soundsGo").Count);
    }
}
