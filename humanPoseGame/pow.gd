extends Node3D

@onready var timer = $Timer
@onready var sprite = $particleSprite

var velocity: Vector3 = Vector3.ZERO
var gravity: float = -9.8

# Called when the node enters the scene tree for the first time.
func _ready():
	sprite.modulate.a = 1.2
	timer.start()
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	# Apply gravity to velocity
	velocity.y += gravity * delta
	
	# Update position based on velocity
	self.position += velocity * delta
	
	# Scale and fade logic
	self.scale.x += 5.0 * delta
	self.scale.y += 5.0 * delta
	sprite.modulate.a -= 0.02 * (delta * 60)
	
	if timer.is_stopped():
		queue_free()
	pass

# Set the initial velocity
func set_velocity(new_velocity: Vector3) -> void:
	velocity = new_velocity

# Set the starting scale
func set_starting_scale(new_scale: Vector3) -> void:
	self.scale = new_scale

# Do not use this on the frame it is created
func set_modulate(color: Color):
	sprite.modulate = color
