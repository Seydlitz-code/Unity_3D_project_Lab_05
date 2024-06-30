using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
	// 현재 플레이어의 손에 장착된 무기
	[SerializeField]
	private Hand currentHand;

	// 공격중인지 여부 판단
	private bool isAttack = false;
	private bool isSwing = false;
	private RaycastHit hitInfo;

	void Update()
	{

		TryAttack();

	}

	private void TryAttack()
	{
		if (Input.GetButton("Fire1"))
		{
			if (!isAttack)
			{
				StartCoroutine(AttackCoroutine());
			}
		}
	}

	IEnumerator AttackCoroutine()
	{
		isAttack = true;
		currentHand.anim.SetTrigger("Attack");

		yield return new WaitForSeconds(currentHand.attackDelayA);
		isSwing = true;

		StartCoroutine(HitCoroutine());

		yield return new WaitForSeconds(currentHand.attackDelayB);
		isSwing = false;


		yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
		isAttack = false;
	}

	IEnumerator HitCoroutine()
	{
		while (isSwing)
		{
			if (CheckObject())
			{
				isSwing = false;
				Debug.Log(hitInfo.transform.name);
			}
			yield return null;
		}
	}

	private bool CheckObject()
	{
		if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
		{
			return true;
		}
		return false;
	}
}
