﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

[Serializable]
public class UpdateClientMaps : MonoBehaviour
{

    public bool autoUpdateMap = true;
    public bool manualUpdateMap = false;

    public List<DMSelectable> sendableObjects = new List<DMSelectable>();

    SocketIOComponent socket;
    Map map;    //Need to wrap json in container, cannot send list

    // Start is called before the first frame update
    void Start()
    {
        socket = GetComponent<SocketIOComponent>();

        StartCoroutine("UpdateMap");
        map = new Map();
    }

    private void Update()
    {
        if (manualUpdateMap)
        {
            SendMapToClients();
            manualUpdateMap = false;
        }
    }

    //Send automatically
    private IEnumerator UpdateMap()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (autoUpdateMap)
            {
                SendMapToClients();
            }
        }
    }

    void SendMapToClients()
    {
        ConstructMapFromObjects();

        var json_map = JSONObject.Create(JsonUtility.ToJson(map));
        socket.Emit("dm-update-map", json_map);
    }

    private void ConstructMapFromObjects()
    {
        //Plz please make this better

        map.tiles.Clear();
        sendableObjects = new List<DMSelectable>(FindObjectsOfType<DMSelectable>());
        foreach (DMSelectable selectable in sendableObjects)
        {
            map.tiles.Add(selectable.tile);
        }

    }

}

[Serializable]
public class Map
{
    public List<TileBlock> tiles = new List<TileBlock>();
    public Map()
    {
        tiles = new List<TileBlock>();
    }

    public void AddTile(TileBlock tile)
    {
        tiles.Add(tile);
    }

    public void RemoveTile(TileBlock tile)
    {
        tiles.Remove(tile);
    }
}

