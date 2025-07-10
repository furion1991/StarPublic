import cn from 'classnames'

import classes from './PriceWithCurrency.module.scss'

type PriceWithCurrencyProps = {
  children: React.ReactNode
  className?: string
  orange?: boolean
  image?: {
    width?: number
    height?: number
  }
}

export const PriceWithCurrency = ({
  children,
  className,
  orange,
  image
}: PriceWithCurrencyProps) => {
  return (
    <span
      className={cn(classes.priceWithCurrency, className, {
        [classes.orange]: orange
      })}
    >
      {children}

      <svg
        width={image?.width || 20}
        height={image?.height || 20}
        viewBox='0 0 19 19'
        fill='none'
        xmlns='http://www.w3.org/2000/svg'
      >
        <g id='Group 191'>
          <path
            id='Vector 27'
            d='M10.0316 12.8125H7.30664L9.41058 18.5007L12.1963 10.1436L10.0316 12.8125Z'
            strokeWidth='0.270589'
          />
          <path
            id='Vector 33'
            d='M12.9291 9.70624L12.0426 12.5418L17.9349 12.2768L11.1705 7.271L12.9291 9.70624Z'
            strokeWidth='0.270589'
          />
          <path
            id='Vector 34'
            d='M10.6306 6.18903L13.397 8.21208L14.69 2.1295L7.9244 7.13563L10.6306 6.18903Z'
            strokeWidth='0.270589'
          />
          <path
            id='Vector 35'
            d='M6.87607 7.20628L9.471 5.37097L4.13394 2.12977L6.70458 10.2474L6.87607 7.20628Z'
            strokeWidth='0.270589'
          />
          <path
            id='Vector 36'
            d='M6.30034 11.0594L5.54878 8.48309L0.887631 12.1775L9.41171 12.2768L6.30034 11.0594Z'
            strokeWidth='0.270589'
          />
        </g>
      </svg>
    </span>
  )
}
