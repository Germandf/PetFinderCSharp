# PetFinderCSharp
**PetFinder** es un sitio web que permite reportar mascotas perdidas junto a su última ubicación, fotos, datos de contacto, etc.
**PetFinderCSharp** es una recreación del sitio original en .NET 5.0, utilizando C# como lenguaje principal, Blazor Server Side, Entity Framework, entre otros.

## Pasos para configurar la base de datos
- Agregar variable de entorno SQLServerPetFinder con la ruta a utilizar
- Para crearla o actualizarla: **Update-Database** en Package Manager Console

## Pasos para configurar JWT
- Agregar variable de entorno PetFinderJWTSecret con un código secreto generado al azar

## Cuenta de administrador
- La primer cuenta en registrarse una vez creada la base de datos, se proclamará automáticamente como administrador. Luego podrá ascender a otros usuarios a administrador si así lo desea.

## Integrantes
Los integrantes del equipo son:
<table>
	<tr>
		<td align="center" width="120">
			<a src="https://github.com/JHuertasDev"><img src="https://avatars3.githubusercontent.com/u/47471125?s=460&u=cc5d454568a2e5267141935335edde9537722509&v=4" alt="Huertas José" width="40" height="40" /></a><br/><a href="https://github.com/JHuertasDev">Huertas José</a>
		</td>
		<td align="center" width="120">
			<a src="https://github.com/Germandf"><img src="https://avatars0.githubusercontent.com/u/69018178?s=460&u=a3d62d0ff3fe9c5934b51e5133753ace8be78a9c&v=4" alt="De Francesco Germán" width="40" height="40" /></a><br/><a href="https://github.com/Germandf">De Francesco Germán</a>
		</td>
	</tr>
</table>

**Ir a [PetFinder](https://github.com/Germandf/PetFinder) para ver el proyecto original**