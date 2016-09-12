Shader "Cg basic shader" { // defines the name of the shader 
   SubShader { // Unity chooses the subshader that fits the GPU best
      Pass { // some shaders require multiple passes

      	 cull off

         CGPROGRAM // here begins the part in Unity's Cg

         #pragma vertex vert 
            // this specifies the vert function as the vertex shader 
         #pragma fragment frag
            // this specifies the frag function as the fragment shader

         struct vertexInput {
         	float4 vertex : POSITION;
            float4 color : COLOR;
         };

         struct vertexOutput {
         	float4 pos : SV_POSITION;
         	float4 pos_in_world : TEXCOORD0;
            float4 color : COLOR;
         };

         vertexOutput vert(vertexInput input)
            // vertex shader 
         {
         	vertexOutput output;

         	output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
         	output.pos_in_world = input.vertex;
            output.color = input.color;
            return output;

               // this line transforms the vertex input parameter 
               // vertexPos with the built-in matrix UNITY_MATRIX_MVP
               // and returns it as a nameless vertex output parameter 
         }

         float4 frag(vertexOutput input) : COLOR // fragment shader
         {
//         	if (input.pos_in_world.y < 100) {
//         		return float4(204.0/255.0, 102.0/255.0, 0, 1.0);
//         	}else if (input.pos_in_world.y >= 100 && input.pos_in_world.y < 250){
//         		return float4(0, 102.0/255.0, 0, 1.0);
//         	}else if (input.pos_in_world.y >= 250 && input.pos_in_world.y < 400){
//         		return float4(178.0f/255.0, 255.0f/255.0, 102.0f/255.0, 1.0);
//         	}else {
//         		return float4(1.0, 1.0, 1.0, 1.0);
//         	}

            return input.color;
            
               // this fragment shader returns a nameless fragment
               // output parameter (with semantic COLOR) that is set to
               // opaque red (red = 1, green = 0, blue = 0, alpha = 1)
         }

         ENDCG // here ends the part in Cg 
      }
   }
}
