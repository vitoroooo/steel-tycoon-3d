"""The sheet studio: a paper backdrop, one key sun, soft fill, and a long lens."""
import math
import bpy
from mathutils import Vector
from . import paint


def light(scene):
    world = bpy.data.worlds.new('Kertas')
    world.use_nodes = True
    world.node_tree.nodes['Background'].inputs[0].default_value = paint.colour('cream')
    world.node_tree.nodes['Background'].inputs[1].default_value = .8
    scene.world = world
    sun = bpy.data.objects.new('Matahari', bpy.data.lights.new('Matahari', 'SUN'))
    sun.data.energy, sun.data.angle = 3.2, math.radians(2)
    sun.rotation_euler = (math.radians(52), 0, math.radians(-38))
    scene.collection.objects.link(sun)


def floor(scene):
    bpy.ops.mesh.primitive_plane_add(size=60)
    plane = bpy.context.object
    plane.is_shadow_catcher = True


def camera(scene, target, distance, yaw_deg=24, pitch_deg=9, lens=85):
    cam = bpy.data.objects.new('Kamera', bpy.data.cameras.new('Kamera'))
    cam.data.lens = lens
    yaw, pitch = math.radians(yaw_deg), math.radians(pitch_deg)
    offset = Vector((math.sin(yaw) * math.cos(pitch), -math.cos(yaw) * math.cos(pitch), math.sin(pitch)))
    cam.location = Vector(target) + offset * distance
    cam.rotation_euler = (Vector(target) - cam.location).to_track_quat('-Z', 'Y').to_euler()
    scene.collection.objects.link(cam)
    scene.camera = cam


def render(path, width=1600, height=1000, samples=48):
    scene = bpy.context.scene
    scene.render.engine = 'CYCLES'
    scene.cycles.samples, scene.cycles.use_denoising = samples, True
    scene.cycles.transparent_max_bounces = 16
    scene.render.film_transparent = True
    scene.view_settings.view_transform = 'Standard'
    scene.render.resolution_x, scene.render.resolution_y = width, height
    scene.render.filepath = str(path)
    bpy.ops.render.render(write_still=True)
