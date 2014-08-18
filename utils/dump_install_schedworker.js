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
printPath(schedworkerDir)
out.close();

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