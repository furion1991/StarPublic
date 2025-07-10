import Image from 'next/image'
import cn from 'classnames'

import { Button, PriceWithCurrency } from '@/shared/ui'

import classes from './CasePreview.module.scss'

type CasePreviewProps = {
  name: string
  image?: string
  imageType?: 'FirstCategory' | 'SecondCategory'
  price: number
  oldPrice: number
  openLimit: number
  openedCasesNumber: number
}

export const CasePreview = ({
  name,
  image = '/placeholders/case.png',
  imageType = 'FirstCategory',
  price,
  oldPrice,
  openLimit,
  openedCasesNumber
}: CasePreviewProps) => {
  const limitFillInPercentage = Number(Number((openedCasesNumber / openLimit) * 100).toFixed(2))

  return (
    <div className={classes.casePreview}>
      <div
        className={cn(classes.image, {
          [classes.secondCategory]: imageType === 'SecondCategory',
          [classes.firstCategory]: imageType === 'FirstCategory',
        })}
      >
        <Image
          src={image}
          alt={`Кейс ${name}`}
          fill
          style={{
            objectFit: 'contain'
          }}
        />
      </div>

      <div className={classes.caseOpenLimits}>
        <span>
          {openedCasesNumber} / {openLimit}
        </span>

        <div
          className={cn(classes.fillGradient, {
            [classes.fillGradientFull]: limitFillInPercentage === 100
          })}
          style={{
            // if percent lower than 7 styles will broke
            display: limitFillInPercentage > 7 ? 'block' : 'none',
            width: `${limitFillInPercentage}%`
          }}
        />
      </div>

      <h4 className={classes.name}>{name}</h4>

      <div className={classes.price}>
        <Button className={classes.priceBtn}>
          <PriceWithCurrency
            image={{
              width: 18,
              height: 18
            }}
          >
            {price}
          </PriceWithCurrency>
        </Button>

        <div className={classes.oldPrice}>
          <PriceWithCurrency
            image={{
              width: 12,
              height: 12
            }}
          >
            {oldPrice}
          </PriceWithCurrency>
        </div>
      </div>
    </div>
  )
}
