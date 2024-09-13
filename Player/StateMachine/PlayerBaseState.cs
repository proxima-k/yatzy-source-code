


public abstract class PlayerBaseState : BaseState {
    protected PlayerStateManager playerStateManager;

    protected PlayerBaseState(PlayerStateManager playerStateManager) {
        this.playerStateManager = playerStateManager;
    }
}
