using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager {

	private static GameManager _instance;
    public enum GameState {MENU, START, BEGIN, ENDGAME};
    public GameState gameState;

    public static GameManager GetInstance() {
		if(_instance == null) {
			_instance = new GameManager();
		}

		return _instance;
	}

    private GameManager() {
		gameState = GameState.MENU;
	}
}
