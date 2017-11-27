var gulp = require("gulp");
var uglify = require('gulp-uglify');

gulp.task('uglify', function () {
    gulp.src('**/*.js')
        .pipe(uglify())
        .pipe(gulp.dest('minifiedjs'));
});