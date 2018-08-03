import { Component, Inject } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";

@Component({
    selector: "quiz",
    templateUrl: './quiz.component.html',
    styleUrls: ['./quiz.component.css']
})

export class QuizComponent {
    quiz: Quiz;

    constructor(private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) {
        //create an empty object from the Quiz interface
        this.quiz = <Quiz>{};

        var id = +this.activatedRoute.snapshot.params["id"];
        console.log(id);

        if (id) {
            //To-Do load the quiz using server-side API
            var url = this.baseUrl + "api/quiz/" + id;
            this.http.get<Quiz>(url).subscribe(result => {
                this.quiz = result;
            }, error => console.log(error));
        }
        else {
            console.log("Invalid id: routing back to home...");
            this.router.navigate(["home"]);
        }
	}
	onEdit() {
		this.router.navigate(["quiz/edit", this.quiz.Id]);
	}
}