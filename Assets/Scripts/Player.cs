﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Character
{
    public GameManager MyGameManager;
    public CameraController MyCameraController;
    public Weapon MyWeapon;
    public Vector2 CheckPoint;
    public List<Key> Keys;
    public int SecretItemsAmount;
    public int AmmoAmount;
    public int CoinsAmount;

    private float horizontal;
    private float vertical;

    private void Start()
    {
        CheckPoint = transform.position;
        MyGameManager.SetAmmoAmount(AmmoAmount);
    }

    private void Update()
    {
        MyWeapon.Aim(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (!MyCameraController.IsMoving)
        {
            Flip();
            MyRigidbody2D.velocity = new Vector2(horizontal * MovementSpeed, vertical * MovementSpeed);
        }
        else
            MyRigidbody2D.velocity = Vector2.zero;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && AmmoAmount > 0)
        {
            MyWeapon.Shoot();
            AmmoAmount--;
            MyGameManager.SetAmmoAmount(AmmoAmount);
        }
    }

    private void Flip()
    {
        if (horizontal > 0 && !FacingLeft || horizontal < 0 && FacingLeft)
            ChangeDirection();
    }

    public override void KillCharacter()
    {
        if (!MyGameManager.BossFight)
        {
            MyGameManager.StartCoroutine(MyGameManager.DeathAnimation(this, ResetPlayer));
            gameObject.SetActive(false);
        }
        else
            ReloadLevel();
    }

    private void ReloadLevel()
    {
        gameObject.SetActive(false);
        MyGameManager.StartCoroutine(MyGameManager.DeathAnimation(this,
            () => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single)));
    }

    private void ResetPlayer()
    {
        gameObject.SetActive(true);
        RemoveKeys();
        transform.position = CheckPoint;
    }

    private void RemoveKeys()
    {
        for (int i = 0; i < Keys.Count; i++)
        {
            Key key = Keys[i];
            if (key.PlayerCheckpoint == CheckPoint)
            {
                key.ResetKey();
                Keys.Remove(key);
            }
        }
    }

    public void PickUpCoin()
    {
        CoinsAmount++;
        MyGameManager.SetCoinsAmount(CoinsAmount);
    }

    public void SetCheckPoint(Vector2 pos)
    {
        CheckPoint = pos;
        AmmoAmount += 1;
        MyGameManager.SetAmmoAmount(AmmoAmount);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "Enemy")
            KillCharacter();
        else if (coll.tag == "Coin")
        {
            coll.gameObject.SetActive(false);
            PickUpCoin();
        }
        else if (coll.tag == "SecretItem")
        {
            coll.gameObject.SetActive(false);
            SecretItemsAmount++;
        }
    }
}