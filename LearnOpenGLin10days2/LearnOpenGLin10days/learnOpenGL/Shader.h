#pragma once

#include<glad/glad.h>

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

#include<string>
#include<fstream>
#include<sstream>
#include<iostream>

class Shader
{
	public:
		// program ID
		unsigned int ID;

		Shader(const char* vertexPath, const char* fragmentPath);

		// use current program
		void use();

		void setBool(const std::string& name, bool value) const;
		void setInt(const std::string& name, int value) const;
		void setFloat(const std::string& name, float value) const;
		void setMat4(const std::string& name, glm::mat4 value) const;
		void setVec3(const std::string& name, glm::vec3 value) const;
		void setVec3(const std::string& name, float v0, float v1, float v2) const;
};

