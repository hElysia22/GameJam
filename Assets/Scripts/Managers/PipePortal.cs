using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PipePortal : MonoBehaviour
{
    [Header("配对的另一个管道")]
    public PipePortal targetPipe;

    public enum PipeType
    {
        DownEnter,
        RightEnter,
        LeftEnter
    }
    [Header("管道输入方向")]
    public PipeType pipeDir;

    [Header("传送参数")]
    public float transferDelay = 0.4f;
    public float shrinkSpeed = 6f;
    public float popSpeed = 8f;
    public float shrinkTargetScale = 0f;
    public Vector2 exitOffset = new Vector2(0, 1.2f);
    public Vector2 pipeInsideOffset = new Vector2(0, -0.4f);

    private bool _isTransferring;
    private Transform _player;
    private Rigidbody2D _playerRb;
    private PlayerManager _playerManager;
    private Vector3 _playerOriginalScale;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Transform enterPlayer = other.transform;
        // 关键：进入前先解绑旧事件，防止重复订阅
        if (_playerManager != null)
        {
            _playerManager.MovePerformedEvent -= OnPlayerMoveInput;
        }

        // 覆盖玩家数据
        _player = enterPlayer;
        _playerRb = other.GetComponent<Rigidbody2D>();
        _playerManager = other.GetComponent<PlayerManager>();
        _playerOriginalScale = _player.localScale;

        // 重新订阅，保证只绑定一次
        if (_playerManager != null)
        {
            _playerManager.MovePerformedEvent += OnPlayerMoveInput;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || _player == null) return;

        // 无论是否传送中，都解绑事件！杜绝事件残留
        if (_playerManager != null)
        {
            _playerManager.MovePerformedEvent -= OnPlayerMoveInput;
        }

        // 只有不在传送状态，才清空引用；传送中保留引用完成动画
        if (!_isTransferring)
        {
            _player = null;
            _playerRb = null;
            _playerManager = null;
        }
    }

    private void OnPlayerMoveInput(InputAction.CallbackContext ctx)
    {
        // 传送中直接拦截，禁止再次触发传送
        if (_isTransferring || _player == null) return;

        Vector2 moveDir = ctx.ReadValue<Vector2>();
        bool triggerTeleport = false;

        switch (pipeDir)
        {
            case PipeType.DownEnter:
                triggerTeleport = moveDir.y < -0.5f;
                break;
            case PipeType.RightEnter:
                triggerTeleport = moveDir.x > 0.5f;
                break;
            case PipeType.LeftEnter:
                triggerTeleport = moveDir.x < -0.5f;
                break;
        }

        if (triggerTeleport)
        {
            StartCoroutine(TeleportCoroutine());
        }
    }

    private IEnumerator TeleportCoroutine()
    {
        _isTransferring = true;
        if (_player == null || _playerRb == null || targetPipe == null)
        {
            _isTransferring = false;
            yield break;
        }

        _playerRb.simulated = false;
        Vector2 innerPos = (Vector2)transform.position + pipeInsideOffset;

        while (_player.localScale.x > shrinkTargetScale + 0.01f)
        {
            if (_player == null)
            {
                _isTransferring = false;
                yield break;
            }
            _player.localScale = Vector3.Lerp(_player.localScale, Vector3.one * shrinkTargetScale, shrinkSpeed * Time.deltaTime);
            _player.position = Vector2.Lerp(_player.position, innerPos, shrinkSpeed * Time.deltaTime);
            yield return null;
        }
        if (_player != null)
            _player.localScale = Vector3.one * shrinkTargetScale;

        yield return new WaitForSeconds(transferDelay);

        if (_player == null || targetPipe == null)
        {
            _isTransferring = false;
            yield break;
        }

        Vector2 outPos = (Vector2)targetPipe.transform.position + targetPipe.exitOffset;
        _player.position = outPos;

        while (_player.localScale.x < _playerOriginalScale.x - 0.01f)
        {
            if (_player == null)
            {
                _isTransferring = false;
                yield break;
            }
            _player.localScale = Vector3.Lerp(_player.localScale, _playerOriginalScale, popSpeed * Time.deltaTime);
            float bounce = Mathf.Sin(Time.time * 20f) * 0.05f;
            _player.position = outPos + new Vector2(0, bounce);
            yield return null;
        }
        if (_player != null)
            _player.localScale = _playerOriginalScale;

        if (_playerRb != null)
        {
            _playerRb.simulated = true;
            _playerRb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(0.2f);

        // 传送完全结束，清空所有引用，彻底断开关联
        if (_playerManager != null)
        {
            _playerManager.MovePerformedEvent -= OnPlayerMoveInput;
        }
        _player = null;
        _playerRb = null;
        _playerManager = null;
        _isTransferring = false;
    }

    private void OnDestroy()
    {
        if (_playerManager != null)
        {
            _playerManager.MovePerformedEvent -= OnPlayerMoveInput;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        Gizmos.DrawWireSphere((Vector2)transform.position + pipeInsideOffset, 0.15f);

        if (targetPipe != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetPipe.transform.position);
            Gizmos.DrawWireSphere(targetPipe.transform.position + (Vector3)targetPipe.exitOffset, 0.2f);
        }
    }
}
