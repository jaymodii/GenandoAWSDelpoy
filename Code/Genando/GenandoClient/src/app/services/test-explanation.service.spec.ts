import { TestBed } from '@angular/core/testing';

import { TestExplanationService } from './test-explanation.service';

describe('TestExplanationService', () => {
  let service: TestExplanationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TestExplanationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
