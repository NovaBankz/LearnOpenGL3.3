#version 330 core

out vec4 FragColor;

in VS_OUT
{
	vec3 FragPos;
	vec3 Normal;
	vec2 TexCoords;
	vec4 LightSpaceFragPos;
} fs_in;

uniform sampler2D diffuseTexture;
uniform sampler2D shadowMap;
uniform vec3 lightPos;
uniform vec3 viewPos;

float ShadowCalculation(vec4 lightSpaceFragPos)
{
	vec3 projCoords = lightSpaceFragPos.xyz / lightSpaceFragPos.w;
	// adjust NDC [-1, 1] to texture [0, 1]
	projCoords = projCoords * 0.5f + 0.5f; 
	// percentage closer function

	float closestDepth = texture(shadowMap, projCoords.xy).r;
	float currentDepth = projCoords.z;
	float shadow = 0;
	float shadowbias = max(0.05 * (1 - dot(normalize(fs_in.Normal), normalize(lightPos - fs_in.FragPos))), 0.005f);
	vec2 texelSize = vec2(1.0f) / textureSize(shadowMap, 0);
	if (currentDepth > 1)
	{
		shadow = 0;
	}
	else
	{
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r;
				shadow += currentDepth - shadowbias > pcfDepth ? 1.0f: 0.0f;
			}
		}
		shadow /= 9;
	}
	return shadow;
}

void main()
{            
	vec3 color = texture(diffuseTexture, fs_in.TexCoords).rgb;
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightColor = vec3(1.0);
    // ambient
    vec3 ambient = 0.15 * color;
    // diffuse
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * lightColor;
    // specular
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * lightColor;    
    // calculate shadow
    float shadow = ShadowCalculation(fs_in.LightSpaceFragPos);       
    vec3 lighting = ambient + ((1.0 - shadow) * (diffuse + specular)) * color;    
    
    FragColor = vec4(lighting, 1.0);
}

