
# Guerra_Agentes_ML
Espacio orientado a generar trazabilidad sobre el proyecto final de DataAnalytics - IronHack.

Generación de un combate en tiempo real haciendo uso de entidades que previamente han sido entrenadas por un modelo de redes neuronales. El objetivo de este proyecto se alcanzará haciendo uso de:

 - Python 3.7.9
 - cudatoolkit 10.2.89
 - mlagents 0.22.0
 - mlagents-envs 0.22.0
 - numpy 1.18.5
 - pytorch 1.7
 - pyyaml 5.3.1
 - tensorboard 2.4.0
 - Unity 2019.4.10f1

La idea es que el proyecto crezca a medida que vaya avanzando. Dado que no conozco los tiempos que pueda tomar el entrenar los agentes necesarios para este proyecto planteare una serie de pasos tentativos que me gustaría tomar durante el desarrollo del proyecto:

 1. Entrenamiento de tanque: He considerado el tanque dividirlo en dos agentes distintos.
		 - Cuerpo del tanque: Padre de la entidad, encargado de moverse en el terrano. 
		 - Cañon del tanque: Hijo de la entidad, encargado de disparar a los objetivos.
 2. Entrenamiento de un barco: Tentativamente puede ser la siguiente entidad a entrenar. El reto estaría en entrenar un barco que aprenda a moverse en el agua. Se podría usar el mismo cañon de tanque entrenado previamente.
