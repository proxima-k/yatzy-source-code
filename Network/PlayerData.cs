using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {
    
    public ulong ClientID => _clientId;
    private ulong _clientId;
    
    public FixedString32Bytes PlayerName => _playerName;
    private FixedString32Bytes _playerName;
    
    public Color PlayerColor => _playerColor;
    private Color _playerColor;
    
    public PlayerData(ulong clientId, Color playerColor) {
        _clientId = clientId;
        _playerName = new FixedString32Bytes(clientId.ToString());
        _playerColor = playerColor;
    }
    
    public void SetPlayerName(FixedString32Bytes playerName) {
        _playerName = new FixedString32Bytes(playerName);
    }
    
    public void SetPlayerColor(Color playerColor) {
        _playerColor = playerColor;
    }
    
    public bool Equals(PlayerData other) {
        return _clientId == other._clientId;
    }
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref _clientId);
        serializer.SerializeValue(ref _playerName);
        serializer.SerializeValue(ref _playerColor);
    }
}
