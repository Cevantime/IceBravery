shader_type canvas_item;

uniform vec4 fire_color_light:hint_color = vec4(1.0, 0.5, 0., 1.0);
uniform vec4 fire_color_dark:hint_color = vec4(0.65, 0., 0., 1.0);

uniform int colors = 6;
uniform float scale_factor = 5.;
uniform sampler2D noise_texture:hint_black;
uniform vec2 resolution_factor;
uniform float screen_ratio;
uniform vec2 camera_offset = vec2(0.);

varying vec2 world_pos;

void vertex() {
	world_pos = (WORLD_MATRIX * vec4(VERTEX, 1.0, 1.0)).xy;
}


float rand(vec2 coord){
	// prevents randomness decreasing from coordinates too large
	coord = mod(coord, 10000.0);
	// returns "random" float between 0 and 1
	return fract(sin(dot(coord, vec2(12.9898,78.233))) * 43758.5453);
}

vec2 rand2( vec2 coord ) {
	// prevents randomness decreasing from coordinates too large
	coord = mod(coord, 10000.0);
	// returns "random" vec2 with x and y between 0 and 1
    return fract(sin( vec2( dot(coord,vec2(127.1,311.7)), dot(coord,vec2(269.5,183.3)) ) ) * 43758.5453);
}


float perlin_noise(vec2 coord) {
	vec2 i = floor(coord);
	vec2 f = fract(coord);
	
	// 4 corners of a rectangle surrounding our point
	// must be up to 2pi radians to allow the random vectors to face all directions
	float tl = rand(i) * 6.283;
	float tr = rand(i + vec2(1.0, 0.0)) * 6.283;
	float bl = rand(i + vec2(0.0, 1.0)) * 6.283;
	float br = rand(i + vec2(1.0, 1.0)) * 6.283;
	
	// original unit vector = (0, 1) which points downwards
	vec2 tlvec = vec2(-sin(tl), cos(tl));
	vec2 trvec = vec2(-sin(tr), cos(tr));
	vec2 blvec = vec2(-sin(bl), cos(bl));
	vec2 brvec = vec2(-sin(br), cos(br));
	
	// getting dot product of each corner's vector and its distance vector to current point
	float tldot = dot(tlvec, f);
	float trdot = dot(trvec, f - vec2(1.0, 0.0));
	float bldot = dot(blvec, f - vec2(0.0, 1.0));
	float brdot = dot(brvec, f - vec2(1.0, 1.0));
	
	// putting these values through abs() gives an interesting effect
//	tldot = abs(tldot);
//	trdot = abs(trdot);
//	bldot = abs(bldot);
//	brdot = abs(brdot);
	
	vec2 cubic = f * f * (3.0 - 2.0 * f);
	
	float topmix = mix(tldot, trdot, cubic.x);
	float botmix = mix(bldot, brdot, cubic.x);
	float wholemix = mix(topmix, botmix, cubic.y);
	
	return 0.5 + wholemix;
}


float fbm(vec2 coord){
	int OCTAVES = 4;
	
	float normalize_factor = 0.0;
	float value = 0.0;
	float scale = 0.5;

	for(int i = 0; i < OCTAVES; i++){
		value += perlin_noise(coord) * scale;
		//value += texture(noise_texture, coord * 0.1).x * scale;
		normalize_factor += scale;
		coord *= 2.0;
		scale *= 0.5;
	}
	return value / normalize_factor;
}

void fragment()
{
	vec2 uv = ((world_pos + camera_offset) / resolution_factor) * 0.015;
	
	vec2 size = vec2(48.0, 48.0);
	vec2 noise_uv = uv;
	//vec2 noise_uv = round(uv * size) / size;
	
	noise_uv *= scale_factor;
    vec2 q = vec2(0.);
    q.x = fbm( noise_uv + 0.34*TIME);
    q.y = fbm( noise_uv + vec2(1.0));

    vec2 r = vec2(0.);
    r.x = fbm( noise_uv + 1.0*q + vec2(1.7,9.2)+ 0.60*TIME );
    r.y = fbm( noise_uv + 1.0*q + vec2(8.3,2.8)+ 0.126*TIME);

    float n = fbm(noise_uv+r) ;
	//float n = perlin_noise(noise_uv);
	//float n = length(texture(noise_texture, noise_uv).xy);
	
	vec4 color = mix(fire_color_dark, fire_color_light, n);
	
	color.rgb = round(color.rgb * float(colors)) / float(colors);
	
	COLOR = color;
}