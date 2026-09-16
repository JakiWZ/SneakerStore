const fs = require("fs");
const path = require("path");
const JavaScriptObfuscator = require("javascript-obfuscator");

const srcDir = path.join(__dirname, "js-src");
const outDir = path.join(__dirname, "wwwroot", "js");

const options = {
  compact: true,
  controlFlowFlattening: true,
  controlFlowFlatteningThreshold: 0.75,
  deadCodeInjection: true,
  deadCodeInjectionThreshold: 0.4,
  identifierNamesGenerator: "hexadecimal",
  renameGlobals: false,
  selfDefending: true,
  stringArray: true,
  stringArrayEncoding: ["base64"],
  stringArrayThreshold: 0.75,
  splitStrings: true,
  splitStringsChunkLength: 6,
};

for (const file of fs.readdirSync(srcDir)) {
  if (!file.endsWith(".js")) continue;
  const code = fs.readFileSync(path.join(srcDir, file), "utf8");
  const result = JavaScriptObfuscator.obfuscate(code, options).getObfuscatedCode();
  fs.writeFileSync(path.join(outDir, file), result);
  console.log(`obfuscated ${file}`);
}
