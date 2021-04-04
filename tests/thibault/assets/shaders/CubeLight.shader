shader_type canvas_item;

uniform vec2 center_position;
uniform vec2 resolution_factor;

uniform float a1 = 20.;
uniform float a2 = 70.;
uniform float a3 = 250.;
uniform float a4 = 280.;

varying vec2 world_pos;

void vertex() {
	world_pos = (WORLD_MATRIX * vec4(VERTEX, 1.0, 1.0)).xy / resolution_factor;
}

void fragment()
{
	vec2 size = vec2(float(textureSize(TEXTURE, 0).x), float(textureSize(TEXTURE, 0).y));
	
	vec2 v = world_pos - center_position;
	float a = 2. * atan(v.y/(length(v) + v.x));
//	float pi = 3.14159;
//	float r = (a + pi) / (2. * pi);
	float d = degrees(a) + 180.;
	
	vec4 tex = texture(TEXTURE, UV);
	float alpha = 0.;
	
	if(d < a1) {
		alpha = 0.;
	} else if(d < a2) {
		alpha = tex.a;
	} else if(d < a3) {
		alpha = 0.
	} else if(d < a4) {
		alpha = tex.a;
	}
	vec4 color = tex;
	color.a = alpha;
	COLOR = color;
}