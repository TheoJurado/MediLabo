Ce projet est une solution .NET en architecture microservices visant à aider les médecins à diagnostiquer le diabète de type 2 à partir de l'analyse des notes rédigées sur les patients.
Les services sont conteneurisés à l’aide de Docker.

Fonctionnalités principales :
- Analyse automatisée des notes médicales.
- Détection des facteurs de risque associés au diabète de type 2.
- Authentification des médecins.
- API REST pour interagir avec l'application.
- Architecture basée sur .NET et Docker.

Le projet est composé de plusieurs microservices :
- GatewayOcelot : API Gateway pour centraliser les accès aux services.
- Frontend : Interface de l'API.
- AuthService : Service d'authentification pour les médecins.
- Medilabo : Gestion des patients.
- MediNote : Gestion des notes.
- Risk : Analyse des notes pour détecter les signaux de diabète.
- SQLServer : Stockage des identifiants des médecins.
- MongoDB : Stockage des notes et patients.
Chaque service est un projet .NET isolé avec son propre Dockerfile.

Installation & Lancement :
- Clonez le dépôt : git clone https://github.com/TheoJurado/MediLabo.git
- Lancez tous les services : docker-compose up --build
