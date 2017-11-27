var gulp = require("gulp");
var uglify = require('gulp-uglify');

gulp.task('uglify', function () {
    gulp.src('CheckForUpdateFromAzureCode.html.js')
        .pipe(uglify())
        .pipe(gulp.dest('minifiedjs'));

    gulp.src('scripts/**/*.js')
        .pipe(uglify())
        .pipe(gulp.dest('minifiedjs/scripts'));
});