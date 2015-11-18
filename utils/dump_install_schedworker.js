load("nashorn:mozilla_compat.js");

importPackage(java.lang);
importPackage(java.io);
importPackage(java.util);
importPackage(java.util.concurrent);
importPackage(java.util.zip);

var excludes = ['.so','.sl','.sh', '.dylib'];
var installerFile = new File("install_schedworker.nsi");
var prefix = new File(System.getProperty("user.dir")).getName();

if (!installerFile.exists()) {
	installerFile.createNewFile();
}

var out = new PrintWriter(installerFile);
// dump all files with relative paths to 
var schedworkerDir = new File("schedworker");

println('>> Dumping all relative paths of files inside the ' + schedworkerDir + ' into the nsis ' + installerFile);
printPath(schedworkerDir);
out.close();

// Fix for AGENT-226 - The standalone packaging should set a node logs dir writable by the agent user account
println('>> Replace proactive.home by java.io.tmpdir inside the schedworker\\config\\log\\node.properties');
var nodePropsFile = new File(schedworkerDir, 'config\\log\\node.properties')
replaceInFile('proactive.home', 'java.io.tmpdir', nodePropsFile);

function printPath(file){
	if (file.isDirectory()) {
		out.println("SetOutPath $INSTDIR" + File.separator + file.getPath())
		var files = file.listFiles(new FileFilter({
			accept: function (fff) {
				return fff.isFile();
		}}));
		for (i in files) {
			var filename = files[i].getName();
			var exclude = false;
			for (e in excludes) {
				if (filename.endsWith(excludes[e])){
					exclude = true;
					break;
				}
			}
			if(!exclude){
				out.println("File \""+ prefix + File.separator + files[i].getPath() + "\"");
			}
		}
		var dirs = file.listFiles(new FileFilter({
			accept: function (fff) {
				return fff.isDirectory();
		}}));
		for (i in dirs) {
			printPath(dirs[i]);
		}
	}
}

function replaceInFile(oldstring, newstring, inFile) {
	var reader = new BufferedReader(new FileReader(inFile));
	var copyFile = File.createTempFile('prefix','blabla');
	var writer = new PrintWriter(new FileWriter(copyFile));
	var line = null;
	while ((line = reader.readLine()) != null) {
		writer.println(line.replaceAll(oldstring, newstring));
	}
	reader.close();
	writer.close();	
	inFile['delete']();
	copyFile.renameTo(inFile);
}