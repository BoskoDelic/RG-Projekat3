#version 330 core
out vec4 FragColor;

struct Material {
    float shininess;
};

struct DirLight{
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

};

struct PointLight {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    vec3 position;

    float constant;
    float linear;
    float quadratic;
};

in vec2 TexCoord;
in vec3 Normal;
in vec3 FragPos;

uniform sampler2D texture1;
uniform sampler2D texture2;
uniform sampler2D textureSpecular;

uniform vec3 viewPos;
uniform Material material;
uniform PointLight pointLight;
uniform DirLight dirLight;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);


void main(){

    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    vec3 result = CalcDirLight(dirLight, norm, viewDir);
    result += CalcPointLight(pointLight, norm, FragPos, viewDir);
    FragColor = vec4(result, 1.0f) * mix(texture(texture1, TexCoord), texture(texture2, TexCoord), 0.1);

}

vec3 CalcDirLight (DirLight light, vec3 normal, vec3 viewDir){

    vec3 lightDir = normalize(light.direction);

    float diff = max(dot(normal, lightDir), 0.0);

    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    vec3 ambient = light.ambient * mix(texture(texture1, TexCoord).rgb, texture(texture2, TexCoord).rgb, 0.1);
    vec3 diffuse = light.diffuse * diff * mix(texture(texture1, TexCoord).rgb, texture(texture2, TexCoord).rgb, 0.1);
    vec3 specular = light.specular * spec *  texture(textureSpecular, TexCoord).rgb;

    return (ambient + diffuse + specular);

}

vec3 CalcPointLight (PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir){

    vec3 lightDir = normalize(light.position - fragPos);

    float diff = max(dot(normal, lightDir), 0.0);

    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    vec3 ambient = light.ambient * mix(texture(texture1, TexCoord).rgb, texture(texture2, TexCoord).rgb, 0.1);
    vec3 diffuse = light.diffuse * diff * mix(texture(texture1, TexCoord).rgb, texture(texture2, TexCoord).rgb, 0.1);
    vec3 specular = light.specular * spec * texture(textureSpecular, TexCoord).rgb;

    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;

    return (ambient + diffuse + specular);

}