extends Spatial

export var speed = 4.0

onready var pivot = $Pivot
onready var mesh = $Pivot/MeshInstance
onready var tween = $Tween

signal DiceRolled

func roll(dir):
	# Do nothing if we're currently rolling.
	if tween.is_active():
		return

	## Step 1: Offset the pivot
	pivot.translate(dir*0.5)
	mesh.global_translate(-dir*0.5)

	## Step 2: Animate the rotation
	var axis = dir.cross(Vector3.DOWN)
	tween.interpolate_property(pivot, "transform:basis",
			null, pivot.transform.basis.rotated(axis, PI/2),
			1/speed, Tween.TRANS_QUAD, Tween.EASE_IN)
	tween.start()
	yield(tween, "tween_all_completed")

	## Step3: Finalize movement and reverse the offset
	transform.origin += dir
	var b = mesh.global_transform.basis  ## Save the rotation
	pivot.transform = Transform.IDENTITY
	mesh.transform.origin = Vector3(0, 0.5, 0)
	mesh.global_transform.basis = b  ## Apply the rotation
	
	emit_signal("DiceRolled")
	


func _on_Tween_tween_step(_object, _key, _elapsed, _value):
	pivot.transform = pivot.transform.orthonormalized()
