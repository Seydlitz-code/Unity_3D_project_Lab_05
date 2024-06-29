using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private float walkSpeed;	//플레이어의 기본 이동 속도
	[SerializeField]
	private float runSpeed;	   //플레이어의 달리기 속도
	[SerializeField]
	private float crouchSpeed;	//앉은 상태에서 플레이어의 이동 속도
	private float applySpeed;	//플레이어의 현재 이동 속도
	[SerializeField]
	private float jumpForce;	//점프 시 적용되는 힘의 크기

	// 플레이어의 현재 상태 확인 변수
	private bool isRun = false;
	private bool isCrouch = false;
	private bool isGround = true;


	[SerializeField]
	private float crouchPosY;	//앉은 상태에서의 플레이어의 높이
	private float originPosY;	//잎반 상태에서의 플레이어의 높이
	private float applyCrouchPosY;	//현재 플레이어의 높이

	private CapsuleCollider capsuleCollider;	//플레이어 오브젝트의 땅 착지 여부


	// 민감도
	[SerializeField]
	private float lookSensitivity;


	// 카메라 한계
	[SerializeField]
	private float cameraRotationLimit;
	private float currentCameraRotationX = 0;


	//필요한 컴포넌트
	[SerializeField]
	private Camera theCamera;

	private Rigidbody myRigid;


	// Use this for initialization
	void Start()
	{
		capsuleCollider = GetComponent<CapsuleCollider>();
		myRigid = GetComponent<Rigidbody>();
		applySpeed = walkSpeed;
		originPosY = theCamera.transform.localPosition.y;
		applyCrouchPosY = originPosY;
	}




	// Update is called once per frame
	void Update()
	{

		IsGround();
		TryJump();
		TryRun();
		TryCrouch();
		Move();
		CameraRotation();
		CharacterRotation();

	}

	private void TryCrouch()	//앉기 입력키 체크 함수
	{
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			Crouch();
		}
	}

	private void Crouch()	//앉기 함수
	{
		isCrouch = !isCrouch;
		if (isCrouch)
		{
			applySpeed = crouchSpeed;
			applyCrouchPosY = crouchPosY;
		}
		else
		{
			applySpeed = walkSpeed;
			applyCrouchPosY = originPosY;
		}

		StartCoroutine(CrouchCoroutine());
	}

	IEnumerator CrouchCoroutine()	//부드러운 앉기 도작 수행
	{

		float _posY = theCamera.transform.localPosition.y;
		int count = 0;

		while (_posY != applyCrouchPosY)
		{
			count++;
			_posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
			theCamera.transform.localPosition = new Vector3(0, _posY, 0);
			if (count > 15)
				break;
			yield return null;
		}
		theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
	}

	private void IsGround()	   //플레이어의 지면착지여부 체크 함수
	{
		isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
	}

	private void TryJump()	//플레이어의 점프 조건 체크 함수
	{
		if (Input.GetKeyDown(KeyCode.Space) && isGround)
		{
			Jump();
		}
	}

	private void Jump()	  //점프 함수
	{
		if (isCrouch)
		{
			Crouch();
		}
		myRigid.velocity = transform.up * jumpForce;
	}


	// 달리기 시도
	private void TryRun()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			Running();
		}
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			RunningCancel();
		}
	}

	private void Running()	//달리기 함수
	{
		if (isCrouch)
		{
			Crouch();
		}
		isRun = true;
		applySpeed = runSpeed;
	}

	private void RunningCancel()	//달리기 취소 함수
	{
		isRun = false;
		applySpeed = walkSpeed;
	}

	private void Move()	   //플레이어의 기본 이동 함수
	{

		float _moveDirX = Input.GetAxisRaw("Horizontal");
		float _moveDirZ = Input.GetAxisRaw("Vertical");

		Vector3 _moveHorizontal = transform.right * _moveDirX;
		Vector3 _moveVertical = transform.forward * _moveDirZ;

		Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

		myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
	}

	private void CharacterRotation()	//카메라 좌우 회전 함수
	{

		float _yRotation = Input.GetAxisRaw("Mouse X");
		Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
		myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
	}

	private void CameraRotation()	//카메라 상하 회전 함수
	{
		float _xRotation = Input.GetAxisRaw("Mouse Y");
		float _cameraRotationX = _xRotation * lookSensitivity;
		currentCameraRotationX -= _cameraRotationX;
		currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

		theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
	}

}
