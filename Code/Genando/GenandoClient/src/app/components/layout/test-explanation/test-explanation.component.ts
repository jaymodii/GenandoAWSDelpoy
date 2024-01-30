import { Component } from '@angular/core';
import { IResponse } from 'src/app/models/shared/response';
import { TestExplanation } from 'src/app/models/test-explanation';
import { TestExplanationService } from 'src/app/services/test-explanation.service';
import { Panel } from 'src/app/shared/models/panel';

@Component({
  selector: 'app-test-explanation',
  templateUrl: './test-explanation.component.html',
  styleUrls: ['./test-explanation.component.scss'],
})
export class TestExplanationComponent {
  panels: Panel[] = [];
  testExplanation!: TestExplanation[];

  constructor(private testExplanationService: TestExplanationService) { }

  ngOnInit(): void {
    this.testExplanationService
      .getTestExplanatipon()
      .subscribe((response: IResponse<TestExplanation[]>) => {
        this.testExplanation = response.data;
        for (let item of this.testExplanation) {
          const panel = {
            name: item.title,
            content: item.description,
            duration: item.duration,
          } as Panel;
          this.panels.push(panel);
        }
      });
  }
}
