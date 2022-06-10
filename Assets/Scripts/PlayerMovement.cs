using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour {

	[SerializeField] float runSpeed = 10f;
	[SerializeField] float jumpSpeed = 10f;
	[SerializeField] float climbSpeed = 5f;
	[SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
	[SerializeField] GameObject bullet;
	[SerializeField] Transform gun;
	Vector2 moveInput;
	Animator myAnimator;
	Rigidbody2D myRigidBody;
	CapsuleCollider2D myBodyCollider;
	BoxCollider2D myFeetCollider;

	float startGravityScale;
	bool isAlive = true;


	void Start() {
		myRigidBody = GetComponent<Rigidbody2D>();
		myAnimator = GetComponent<Animator>();
		myBodyCollider = GetComponent<CapsuleCollider2D>();
		myFeetCollider = GetComponent<BoxCollider2D>();
		startGravityScale = myRigidBody.gravityScale;
	}

	void Update() {
		if (!isAlive) {
			return;
		}
		Run();
		FlipSprite();
		ClimbLadder();
		Die();
	}

	void OnFire(InputValue value) {
		if (!isAlive) {
			return;
		}
		Instantiate(bullet, gun.position, transform.rotation);
	}

	void OnMove(InputValue value) {
		if (!isAlive) {
			return;
		}
		moveInput = value.Get<Vector2>();
	}

	void OnJump(InputValue value) {
		if (!isAlive) {
			return;
		}
		if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
			return;
		}
		if (value.isPressed) {
			myRigidBody.velocity += new Vector2(0f, jumpSpeed);
		}
	}

	void Run() {
		Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidBody.velocity.y);
		myRigidBody.velocity = playerVelocity;
		bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
		myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);

	}

	void FlipSprite() {

		bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
		if (playerHasHorizontalSpeed) {
			transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
		}
	}

	void ClimbLadder() {
		if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
			myRigidBody.gravityScale = startGravityScale;
			myAnimator.SetBool("isClimbing", false);
			return;
		}

		Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, moveInput.y * climbSpeed);
		myRigidBody.gravityScale = 0f;
		myRigidBody.velocity = climbVelocity;

		bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
		myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
	}

	void Die() {
		if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards"))) {
			isAlive = false;
			myAnimator.SetTrigger("Dying");
			myRigidBody.velocity = deathKick;
			FindObjectOfType<GameSession>().ProcessPlayerDeath();
		}
	}



}
